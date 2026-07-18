import { test, expect } from '@playwright/test';

test.describe('Home and navigation', () => {
  test('home page renders and links to key sections', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('heading', { name: /Buy, Sell & Repair/i })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Browse Mobiles' })).toBeVisible();
  });
});

test.describe('Mobiles', () => {
  test('browse and filter mobiles list', async ({ page }) => {
    await page.goto('/mobiles');
    await expect(page.getByRole('heading', { name: 'Mobiles' })).toBeVisible();

    // Wait for the list to load before filtering.
    await page.waitForLoadState('networkidle');

    await page.getByLabel('Brand').fill('Apple');
    await page.waitForLoadState('networkidle');

    const cards = page.locator('a[href^="/mobiles/"]');
    const count = await cards.count();
    for (let i = 0; i < count; i++) {
      await expect(cards.nth(i)).toContainText('Apple');
    }
  });

  test('view mobile detail page', async ({ page }) => {
    await page.goto('/mobiles');
    await page.waitForLoadState('networkidle');

    const firstCard = page.locator('a[href^="/mobiles/"]').first();
    const count = await firstCard.count();
    test.skip(count === 0, 'No mobiles seeded to test detail navigation');

    await firstCard.click();
    await expect(page.getByRole('heading', { level: 5 }).first()).toBeVisible();
    await expect(page.getByText('Interested in this phone?')).toBeVisible();
  });
});

test.describe('Accessories', () => {
  test('accessories page loads with filters', async ({ page }) => {
    await page.goto('/accessories');
    await expect(page.getByRole('heading', { name: 'Accessories' })).toBeVisible();
    await expect(page.getByLabel('Category')).toBeVisible();
    await expect(page.getByLabel('Compatible brand')).toBeVisible();
    await expect(page.getByLabel('Compatible model')).toBeVisible();
  });
});

test.describe('Repair services', () => {
  test('repair services page lists services and inquiry form', async ({ page }) => {
    await page.goto('/repairs');
    await expect(page.getByRole('heading', { name: 'Repair Services' })).toBeVisible();
    await expect(page.getByText('Ask about a repair')).toBeVisible();
  });
});

test.describe('Inquiry submission', () => {
  test('submits an inquiry successfully from the repair services page', async ({ page }) => {
    await page.goto('/repairs');

    await page.getByLabel('Your name').fill('E2E Test User');
    await page.getByLabel('Phone number').fill('555-0199');
    await page.getByLabel('Message (optional)').fill('E2E test inquiry.');
    await page.getByRole('button', { name: 'Send Inquiry' }).click();

    await expect(page.getByText(/Thanks! We've received your inquiry/i)).toBeVisible();
  });

  test('shows validation errors for missing required fields', async ({ page }) => {
    await page.goto('/repairs');

    await page.getByRole('button', { name: 'Send Inquiry' }).click();

    await expect(page.getByText('Name is required')).toBeVisible();
    await expect(page.getByText('Phone number is required')).toBeVisible();
  });
});
