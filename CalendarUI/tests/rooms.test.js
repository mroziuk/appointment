import { test, expect } from "@playwright/test";

test("asAdmin_addRoom", async ({ page }) => {
  await page.goto("https://localhost:5173/login");
  await page.getByPlaceholder("Enter your email here").click();
  await page
    .getByPlaceholder("Enter your email here")
    .fill("admin@admin.admin");
  await page.getByPlaceholder("Enter your email here").press("Tab");
  await page.getByPlaceholder("Enter your password here").fill("Admin123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("link", { name: "Rooms" }).click();
  await page.goto("https://localhost:5173/rooms");
  await page.getByRole("button", { name: "Add" }).click();
  await page.getByLabel("Name").click();
  await page.getByLabel("Name").fill("room 1");
  await page.getByRole("button", { name: "Submit" }).click();
  await expect(page.getByRole("cell", { name: "room" })).toBeVisible();
  await expect(page.getByRole("button", { name: "Delete" })).toBeVisible();
  await expect(page.getByRole("button", { name: "Edit" })).toBeVisible();
  await expect(page.locator("tbody")).toContainText("room 1");
});
