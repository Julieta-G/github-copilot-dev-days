# GitHub Copilot Instructions

Unless a prompt explicitly specifies otherwise, always install and reference the most recent stable release of any software, package, or library.

## Copilot Prompt Control Rules

Use this control block inside every Copilot prompt if Copilot starts generating too much:

```text
Output control:
- Be concise.
- Do not generate long explanations.
- Do not create extra files unless I explicitly ask.
- Do not create page objects.
- Do not generate more than one test at a time.
- Do not refactor unrelated code.
- Do not rewrite the whole project.
- Do not add broad test coverage.
- Do not inspect the repository unless asked.
- Only change the minimum required files.
- After changes, give only:
  1. changed file paths
  2. command to run
  3. short explanation in maximum 5 bullet points
```

## Azure DevOps YAML Generation Rules

When generating Azure DevOps YAML files, use the following structure:

```yaml
trigger:
- feature

stages:
- stage: Launch_End_To_End_Testing
  displayName: Launch end-to-end testing stage
  variables:
    webUiCaUrl: 'http://127.0.0.1:5169'
  jobs:
  - job: Run_End_To_End_Tests
    displayName: Run ShoppingApp.E2ETests
    pool:
      name: Windows-Playwright-Agents
      demands:
      - Agent.Name -equals WIN-PLAYWRIGHT-01
    steps:
    - checkout: self

    - task: DotNetCoreCLI@2
      displayName: Build ShoppingApp.E2ETests
      inputs:
        command: build
        projects: '$(Build.SourcesDirectory)/1_initial_code/test/ShoppingApp.E2ETests/ShoppingApp.E2ETests.csproj'

    - task: DotNetCoreCLI@2
      displayName: Run ShoppingApp.E2ETests
      inputs:
        command: test
        projects: '$(Build.SourcesDirectory)/1_initial_code/test/ShoppingApp.E2ETests/ShoppingApp.E2ETests.csproj'
        publishTestResults: true
      env:
        BASE_URL: $(webUiCaUrl)
```
