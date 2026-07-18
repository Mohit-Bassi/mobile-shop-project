import { test, expect, request, type Browser, type Page } from '@playwright/test';

const PUBLIC_SITE_URL = 'http://localhost:5173';
const API_BASE_URL = 'https://localhost:7152/api/v1';

// One shared, already-authenticated page for the whole file: the refresh token is single-use
// and rotates on every silent refresh, so spinning up a fresh browser context (and therefore a
// fresh silent-refresh-on-mount) per test would invalidate the previous test's session. Logging
// in once here and reusing the same page/context avoids that, and keeps us well under the
// login rate limit (5 attempts/5 min) regardless of how many scenarios this file covers.
test.describe.configure({ mode: 'serial' });

let page: Page;
let browserRef: Browser;

test.beforeAll(async ({ browser }: { browser: Browser }) => {
  browserRef = browser;
  page = await browser.newPage({ ignoreHTTPSErrors: true });
  await page.goto('/login');
  await page.getByLabel('Email').fill('admin@mobileshop.local');
  await page.getByLabel('Password').fill(process.env.E2E_ADMIN_PASSWORD ?? '');
  await page.getByRole('button', { name: 'Sign in' }).click();
  await page.waitForURL('/');
  await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible();
});

test.afterAll(async () => {
  await page.close();
});

test('dashboard shows summary tiles', async () => {
  await expect(page.getByText('Active Mobiles')).toBeVisible();
  await expect(page.getByText('Active Accessories')).toBeVisible();
  await expect(page.getByText('New Inquiries')).toBeVisible();
});

test('create, publish, and hide a mobile listing', async () => {
  const uniqueBrand = `E2EBrand-${Date.now()}`;

  await page.locator('button[aria-label="open navigation menu"]').click();
  await page.getByRole('link', { name: 'Mobiles' }).click();
  await page.waitForURL('**/mobiles');

  // Create as Draft (default) — should not appear on the public site.
  await page.getByRole('button', { name: 'Add Mobile' }).click();
  const createDialog = page.getByRole('dialog');
  await createDialog.getByLabel('Brand').fill(uniqueBrand);
  await createDialog.getByLabel('Model').fill('E2E Model');
  await createDialog.getByLabel('Price').fill('1234');
  await createDialog.getByRole('button', { name: 'Save' }).click();
  await expect(createDialog).toBeHidden();

  const row = page.locator('.MuiDataGrid-row', { hasText: uniqueBrand });
  await expect(row).toBeVisible();
  await expect(row.locator('.MuiChip-label', { hasText: 'Draft' })).toBeVisible();

  const apiContext = await request.newContext({ ignoreHTTPSErrors: true });
  const draftCheck = await apiContext.get(`${API_BASE_URL}/mobiles?brand=${encodeURIComponent(uniqueBrand)}`);
  expect((await draftCheck.json()).items).toHaveLength(0);

  // Publish it — set status to Active via the grid's inline status select.
  await row.locator('[role="combobox"], .MuiSelect-select').first().click();
  await page.getByRole('option', { name: 'Active', exact: true }).click();
  await expect(row.locator('.MuiChip-label', { hasText: 'Active' })).toBeVisible();

  const activeCheck = await apiContext.get(`${API_BASE_URL}/mobiles?brand=${encodeURIComponent(uniqueBrand)}`);
  expect((await activeCheck.json()).items).toHaveLength(1);

  // Confirm it's visible on the public site too.
  const publicPage = await browserRef.newPage({ ignoreHTTPSErrors: true });
  await publicPage.goto(`${PUBLIC_SITE_URL}/mobiles?brand=${encodeURIComponent(uniqueBrand)}`);
  await publicPage.waitForLoadState('networkidle');
  await expect(publicPage.getByText(uniqueBrand)).toBeVisible();
  await publicPage.close();

  // Archive it (soft-delete) — should disappear from the public catalog again.
  await row.getByRole('button').last().click();
  await expect(row.locator('.MuiChip-label', { hasText: 'Draft' })).toBeVisible();

  const afterDeleteCheck = await apiContext.get(`${API_BASE_URL}/mobiles?brand=${encodeURIComponent(uniqueBrand)}`);
  expect((await afterDeleteCheck.json()).items).toHaveLength(0);
  await apiContext.dispose();
});

test('lists a submitted inquiry and updates its status', async () => {
  const uniqueName = `E2E Customer ${Date.now()}`;

  const apiContext = await request.newContext({ ignoreHTTPSErrors: true });
  const submitResponse = await apiContext.post(`${API_BASE_URL}/inquiries`, {
    data: { listingType: 'General', customerName: uniqueName, customerPhone: '555-0123' },
  });
  expect(submitResponse.ok()).toBeTruthy();
  await apiContext.dispose();

  await page.locator('button[aria-label="open navigation menu"]').click();
  await page.getByRole('link', { name: 'Inquiries' }).click();
  await page.waitForURL('**/inquiries');
  await page.waitForLoadState('networkidle');

  const row = page.locator('.MuiDataGrid-row', { hasText: uniqueName });
  await expect(row).toBeVisible();
  await expect(row.getByText('New')).toBeVisible();

  await row.locator('[role="combobox"], .MuiSelect-select').first().click();
  await page.getByRole('option', { name: 'Contacted', exact: true }).click();
  await expect(row.getByText('Contacted')).toBeVisible();
});
