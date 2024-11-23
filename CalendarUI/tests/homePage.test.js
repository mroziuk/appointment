// @ts-check
import { test, expect } from "@playwright/test";

test.describe("Home Page", () => {
  test("should work", async ({ page }) => {
    await page.goto("https://localhost:5173/");

    await expect(page).toHaveTitle("Appointment");
    await expect(
      page.getByRole("heading", {
        name: "Welcome!",
      })
    ).toBeVisible();

    await page.click("text=Log in");
    await expect(page).toHaveTitle("Appointment - Login");
  });
});
