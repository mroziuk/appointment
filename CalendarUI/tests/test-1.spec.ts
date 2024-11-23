import { test, expect } from "@playwright/test";

test("test", async ({ page }) => {
  let uuid = crypto.randomUUID().toString();
  page.on("dialog", async (dialog) => {
    expect(dialog.message()).toEqual(
      "Are you sure you want to delete this item?"
    );
    await dialog.accept();
  });
  await page.goto("https://localhost:5173/login");
  await page.getByPlaceholder("Enter your email here").click();
  await page
    .getByPlaceholder("Enter your email here")
    .fill("admin@admin.admin");
  await page.getByPlaceholder("Enter your email here").press("Tab");
  await page.getByPlaceholder("Enter your password here").fill("Admin123");
  await page.getByRole("button", { name: "Log in" }).click();

  await page.getByRole("link", { name: "Rooms" }).click();
  await page.getByPlaceholder("Search name").click();
  await page.getByPlaceholder("Search name").fill(uuid);
  await expect(page.getByRole("cell", { name: "No data found" })).toBeVisible();
  await expect(page.getByRole("cell")).toContainText("No data found");
  await page.getByRole("button", { name: "Add" }).click();
  await page.getByLabel("Name").click();
  await page.getByLabel("Name").fill(uuid);
  await page.getByRole("button", { name: "Submit" }).click();
  await page.getByPlaceholder("Search name").click();
  await page.getByPlaceholder("Search name").fill(uuid);
  await expect(page.getByRole("cell", { name: uuid })).toBeVisible();
  await page.getByRole("button", { name: "Delete" }).click();
  await expect(page.getByRole("button", { name: "Delete" })).toBeVisible();
  await expect(page.getByRole("button", { name: "Edit" })).toBeVisible();
  await expect(page.locator("tbody")).toContainText(uuid);
  await page.getByRole("button", { name: "Delete" }).click();
  await page.getByPlaceholder("Search name").click();
  await page.getByPlaceholder("Search name").fill(uuid);
  await expect(page.getByRole("cell", { name: "No data found" })).toBeVisible();
  await expect(page.getByRole("cell")).toContainText("No data found");
});
