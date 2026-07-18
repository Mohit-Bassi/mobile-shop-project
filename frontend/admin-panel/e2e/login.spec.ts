import { test, expect } from '@playwright/test';

// Runs with a fresh (unauthenticated) context — not part of the "authenticated" project.
test.describe('Login', () => {
  test('shows an error for wrong credentials', async ({ page }) => {
    await page.goto('/login');
    await page.getByLabel('Email').fill('admin@mobileshop.local');
    await page.getByLabel('Password').fill('definitely-wrong-password');
    await page.getByRole('button', { name: 'Sign in' }).click();

    await expect(page.getByText('Invalid email or password.')).toBeVisible();
    await expect(page).toHaveURL(/\/login$/);
  });

  test('redirects unauthenticated users away from protected pages', async ({ page }) => {
    await page.goto('/mobiles');
    await expect(page).toHaveURL(/\/login$/);
  });
});
