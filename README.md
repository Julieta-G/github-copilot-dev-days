# AI-Powered E2E Testing with Playwright MCP, GitHub Copilot, and Azure DevOps

This repository demonstrates how to build a practical AI-assisted end-to-end testing workflow for a .NET web shop application.

The workflow uses **GitHub Copilot Agent + Playwright MCP** during test authoring and debugging, and **C# Playwright NUnit tests** for deterministic execution locally and in Azure DevOps.

## Repository

```text
https://github.com/Julieta-G/github-copilot-dev-days
```

## What You Will Build

1. Clone the .NET web shop application.
2. Create a safe feature branch for the workshop work.
3. Configure Playwright MCP in Visual Studio Code.
4. Run the .NET web application locally.
5. Use GitHub Copilot Agent with Playwright MCP to inspect the live UI.
6. Create a C# Playwright NUnit E2E test project.
7. Generate focused tests for:
   - product search
   - cart flow
   - short checkout flow
8. Debug failed tests using MCP-grounded browser inspection.
9. Collect screenshots, traces, and test artifacts.
10. Run the same tests in Azure DevOps on a self-hosted Windows agent.

## Architecture

| Layer | Tool | Purpose |
|---|---|---|
| AI browser exploration | GitHub Copilot Agent + Playwright MCP | Allows Copilot to inspect the running UI, understand flows, identify locators, and help generate tests |
| Test implementation | C# Playwright NUnit | Provides deterministic E2E tests that can run locally and in CI |
| CI/CD execution | Azure DevOps self-hosted Windows agent | Runs the final test suite and publishes results |


## Prerequisites

Install or prepare the following before the workshop:

- Visual Studio Code
- Git
- .NET SDK compatible with the application
- Node.js and npm
- PowerShell 7 or later
- GitHub Copilot extension for VS Code
- Playwright MCP extension for VS Code
- Azure DevOps project
- Self-hosted Windows Azure DevOps agent
- Agent pool named `Windows-Playwright-Agents`
- Agent named `WIN-PLAYWRIGHT-01`


# Step 1 — Clone the Repository and Create a Feature Branch

Run:

```powershell
git clone https://github.com/Julieta-G/github-copilot-dev-days.git
cd github-copilot-dev-days-webshop
git checkout -b feature/copilot-playwright-mcp-e2e
code .
```

# Step 2 — Configure Playwright MCP in VS Code

Install Playwright MCP from the VS Code Extensions tab.

1. Open **Extensions** in VS Code.
2. Search for **Playwright MCP**.
3. Install the official Playwright MCP extension.
4. Open **GitHub Copilot Chat**.
5. Select **Agent** mode.
6. Open the tools/configuration icon.
7. Enable the **playwright** MCP server or Playwright browser tools.

# Step 3 — Inspect and Run the .NET Application Locally

Before writing tests, ask Copilot to inspect the application structure.

Prompt:

```text
Inspect this .NET application.
Do not write tests and do not modify files.

Return only:
- startup project
- local run command
- expected local URL
- product listing route
- cart route
- checkout availability
- existing stable selectors, ids, labels, or data-testid attributes

Rules:
- Do not invent selectors, routes, ports, product names, or checkout fields.
- Do not explain the whole codebase.
- Maximum output: 12 bullets.
```

After Copilot identifies the startup project, run the application.

Typical command:

```powershell
dotnet run --project .\1_initial_code\src\ShoppingApp.WebUI\ShoppingApp.WebUI.csproj --urls "http://127.0.0.1:5169"
```

# Step 4 — Use Playwright MCP to Explore the Live Application

Now ask Copilot Agent to use Playwright MCP against the running application.

Prompt:

```text
Use Playwright MCP to explore the running web shop application at:
http://127.0.0.1:5169

Do not modify files.
Return only a compact locator map for:
- product listing navigation
- search input
- first product name
- add-to-cart button
- cart navigation
- cart item row or summary
- checkout button, if it exists
- required checkout fields, if any
- final success state, if any

For each item, give:
- element purpose
- best locator
- fallback locator only if needed
Rules:
- Do not describe the full page.
- Do not invent missing behavior.
- Maximum output: 15 bullets.
```

# Step 5 — Create the C# Playwright NUnit Test Project

Ask Copilot to create a minimal test project.

Prompt:

```text
Create a minimal .NET 10/C# Playwright NUnit E2E test project.

Requirements:
- Use Microsoft.Playwright.NUnit.
- No real test cases yet.
- Add only shared BaseUrl setup.
- Read BASE_URL from environment variable.
- Use http://127.0.0.1:5169 as fallback.
- Add screenshot-on-failure support.
- Do not use TypeScript or JavaScript.
- Do not create page objects.
- Do not add extra helper layers.

Output control:
- Change only required files.
- Return only changed file paths and run commands.
- Maximum explanation: 5 bullets.
```

Run the generated setup commands. Typical commands are:

```powershell
dotnet restore
dotnet build
pwsh .\1_initial_code\test\ShoppingApp.E2ETests\bin\Debug\net10.0\playwright.ps1 install chromium
dotnet test .\1_initial_code\test\ShoppingApp.E2ETests\ShoppingApp.E2ETests.csproj
```

# Step 6 — Add the Product Search Test

Prompt:

```text
Create exactly one C# Playwright NUnit test.
Test name:
ProductSearch_Should_Filter_Products

Scenario:
- Open BaseUrl.
- Navigate to product listing only if needed.
- Read first visible product name.
- Use part of that name as search term.
- Assert at least one matching result remains visible.

Rules:
- Use only MCP-verified selectors.
- Prefer role-based locators.
- No hardcoded product names.
- No invented selectors.
- No Thread.Sleep.
- Do not create page objects.
- Do not create other tests.
- Keep the test short.

Install Playwright browsers to run the tests.

Output:
- changed file path
- test command
- max 3 bullets explaining the test.
```

Run:

```powershell
dotnet test .\1_initial_code\test\ShoppingApp.E2ETests\ShoppingApp.E2ETests.csproj --filter ProductSearch
```

# Step 7 — Add the Cart Flow Test

Prompt:

```text
Create exactly one C# Playwright NUnit test.
Test name:
CartFlow_Should_Add_Product_To_Cart

Scenario:
- Open BaseUrl.
- Capture available product name.
- Add that product to cart.
- Open cart.
- Assert cart is not empty.
- Assert selected product or cart row is visible.
- Assert summary/total only if it exists.

Rules:
- Use only MCP-verified selectors.
- No hardcoded product names.
- No invented button text.
- No Thread.Sleep.
- Do not add remove-cart logic unless it already exists and is trivial.
- Do not create page objects.
- Do not create other tests.

Output:
- changed file path
- test command
- max 3 bullets explaining the test.
```

Run:

```powershell
dotnet test .\1_initial_code\test\ShoppingApp.E2ETests\ShoppingApp.E2ETests.csproj --filter CartFlow
```

# Step 8 — Add the Short Checkout Flow Test

Prompt:

```text
Create exactly one short C# Playwright NUnit checkout test.
Test name:
CheckoutFlow_Should_Complete_Short_Happy_Path

Scenario:
- Add one available product to cart.
- Open cart.
- Assert there is product to checkout.
- Start checkout.
- Fill only required fields that MCP verified.
- Submit.
- Assert the real final success/confirmation state.

Rules:
- If checkout is missing, create no test and report that.
- If checkout only opens a page/dialog, test only that behavior.
- No hardcoded product names.
- No invented fields.
- No Thread.Sleep.
- Save only one final success screenshot.
- Do not create page objects.
- Do not create other tests.

Output:
- changed file path
- test command
- max 3 bullets explaining the test.
```

Run:

```powershell
dotnet test .\1_initial_code\test\ShoppingApp.E2ETests\ShoppingApp.E2ETests.csproj --filter CheckoutFlow
```

# Step 9 — Debug Failed Tests with MCP

When a test fails, do not ask Copilot to blindly make it pass. Ask it to reproduce the failing flow and identify the real cause.

Prompt:

```text
A C# Playwright test is failing.
Use the failure output and Playwright MCP to reproduce only the failing flow.

Find the root cause:
- wrong locator
- wrong navigation
- wrong assertion
- changed app behavior
- missing stable selector

Rules:
- Fix only the failing test or minimal app selector.
- Do not remove assertions.
- Do not use Thread.Sleep.
- Do not weaken the test.
- Do not refactor unrelated code.

Output:
- root cause in 1 sentence
- changed file path
- test command
- max 3 bullets.
```

# Step 10 — Create One Intentional Failure

This step proves that the tests protect real behavior.

Prompt:

```text
Make one tiny reversible UI change that causes exactly one E2E test to fail.

Rules:
- Do not break the whole app.
- Prefer one visible text or button-label change.
- Change only one file.
- Do not modify tests.
- Do not create new files.

Output:
- changed file path
- expected failing test
- why it fails in 1 sentence
- exact revert command.
```

Run the affected test:

```powershell
dotnet test .\1_initial_code\test\ShoppingApp.E2ETests\ShoppingApp.E2ETests.csproj --filter CartFlow
```

Revert the change:

```powershell
git diff
git restore .
```

# Step 11 — Add the Azure DevOps Pipeline

Ask Copilot to create the pipeline.

Prompt:

```text
Create an Azure DevOps YAML pipeline stage for running ShoppingApp.E2ETests .NET Playwright end-to-end tests.
 
The pipeline should follow this structure and behavior:
 
Trigger from only feature branch.
Add a dedicated stage for launching end-to-end testing.
Define a pipeline/stage variable for the Web UI URL (http://127.0.0.1:5169).
Add one job responsible for running the E2E tests and use a self-hosted Azure DevOps agent pool(pool: Windows-Playwright-Agents) and agent (agent: WIN-PLAYWRIGHT-01).
Build the E2E test project before running tests.
Run the E2E tests using dotnet test and pass the deployed Web UI URL into the test run as a test parameter.
Please double check Web UI URL parameter passing syntax.
Publish test results to Azure DevOps.
Use DotNetCoreCLI@2 and the minimal set of commands and tasks required.
```

# Step 12 — Commit and Push

Run:

```powershell
git status
git add .
git commit -m "Add AI-assisted C# Playwright E2E tests with Playwright MCP workflow"
git push -u origin feature/copilot-playwright-mcp-e2e
```