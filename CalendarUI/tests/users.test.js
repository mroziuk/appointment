import { test, expect } from "@playwright/test";

test("asAdmin_seeUsers", async ({ page }) => {
  await page.goto("https://localhost:5173/login");
  await page.getByPlaceholder("Enter your email here").click();
  await page
    .getByPlaceholder("Enter your email here")
    .fill("admin@admin.admin");
  await page.getByPlaceholder("Enter your email here").press("Tab");
  await page.getByPlaceholder("Enter your password here").fill("Admin123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("link", { name: "Users" }).click();
  await expect(page.getByText("Admin", { exact: true }).first()).toBeVisible();
  await expect(page.getByRole("grid")).toContainText("Admin");
  await expect(page.getByRole("grid")).toContainText("admin@admin.admin");
  await expect(page.getByRole("grid")).toContainText("superadmin");
});
