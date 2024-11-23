import { test, expect } from "@playwright/test";

test("test", async ({ page }) => {
  await page.goto("https://localhost:5173/");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByPlaceholder("Enter your email here").click();
  await page
    .getByPlaceholder("Enter your email here")
    .fill("Admin@admin.admin");
  await page.getByPlaceholder("Enter your email here").press("Tab");
  await page.getByPlaceholder("Enter your password here").fill("Admin123");
  await page.getByRole("button", { name: "Log in" }).click();
  await expect(page.getByTestId("ps-sidebar-container-test-id")).toBeVisible();
  await expect(
    page.locator("#root div").filter({ hasText: "<March 2024>" }).nth(1)
  ).toBeVisible();
  await expect(
    page.locator(".root-layout > .content > div").first()
  ).toBeVisible();
});
