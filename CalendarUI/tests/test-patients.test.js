import { test, expect } from "@playwright/test";

test("asAdmin_addPatient_editPatientName_deletePatient", async ({ page }) => {
  await page.goto("https://localhost:5173/login");
  await page.getByPlaceholder("Enter your email here").click();
  await page
    .getByPlaceholder("Enter your email here")
    .fill("admin@admin.admin");
  await page.getByPlaceholder("Enter your email here").press("Tab");
  await page.getByPlaceholder("Enter your password here").fill("Admin123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("link", { name: "Patients" }).click();
  await page.goto("https://localhost:5173/patients");
  await page.getByRole("button", { name: "Add" }).click();
  await page.getByLabel("FirstName").click();
  await page.getByLabel("FirstName").fill("andrzej");
  await page.getByLabel("FirstName").press("Tab");
  await page.getByLabel("LastName").fill("kowalski");
  await page.getByLabel("LastName").press("Tab");
  await page.getByLabel("Phone").fill("888111222");
  await page.getByLabel("Phone").press("Tab");
  await page.getByLabel("Email").fill("andrzej.kowalski@gmail.com");
  await page.getByRole("button", { name: "Submit" }).click();
  await expect(page.getByRole("button", { name: "Delete" })).toBeVisible();
  await expect(page.getByRole("button", { name: "Edit" })).toBeVisible();
  await page.getByRole("button", { name: "Edit" }).click();
  await page.getByLabel("LastName").click();
  await page.getByLabel("LastName").fill("nowak");
  await page.getByRole("button", { name: "Submit" }).click();
  await page.goto("https://localhost:5173/patients");
  await expect(page.getByRole("cell", { name: "nowak" })).toBeVisible();
  await expect(page.locator("tbody")).toContainText("nowak");
  await page.getByRole("button", { name: "Delete" }).click();
  await expect(page.getByRole("cell")).toContainText("No data found");
});
