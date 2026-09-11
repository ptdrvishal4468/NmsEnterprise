# Enterprise NMS CI/CD Delivery Pipeline

## 1. Overview
The Enterprise Network Management System (NMS) utilizes a decoupled two-workflow delivery model engineered on GitHub Actions. It separates continuous verification during active development from immutable semantic releases and environment-gated delivery pipelines.

---

## 2. Workflows & Lifecycle Architecture

### A. Continuous Integration Pipeline (`.github/workflows/ci.yml`)
* **Trigger Conditions**:
  * Push events to `main`, `develop`, and `feature/**` branches.
  * Pull request events targeting `main` and `develop`.
* **Workflow Stages**:
  1. **`build-and-test`**:
     * Sets up the .NET 10 SDK environment.
     * Provisions a containerized Redis service (`redis:7-alpine`) on port `6379` with active health probing (`redis-cli ping`).
     * Verifies solution formatting standard via `dotnet format NmsEnterprise.slnx --verify-no-changes`.
     * Executes Release compilation and static code analysis with Roslyn analyzers enforcing warnings-as-errors (`/p:TreatWarningsAsErrors=true`).
     * Verifies multi-stage container compilation via `docker build -f Nms.Api/Dockerfile -t nms-enterprise-api:ci-build-test .`.
     * Runs the complete test suite across all projects with XPlat Cobertura code coverage collection and TRX logging.
     * Generates and publishes markdown coverage summaries directly to `$GITHUB_STEP_SUMMARY`.
     * Uploads test results and coverage reports as deterministic CI artifacts (`test-and-coverage-results-<SHA>`).
  2. **`security-audit`** (Depends on `build-and-test`):
     * Audits all direct and transitive package references for known security vulnerabilities (`dotnet list NmsEnterprise.slnx package --vulnerable --include-transitive`).
     * Audits all dependencies for deprecation status (`dotnet list NmsEnterprise.slnx package --deprecated --include-transitive`).
  3. **`publish-build-artifacts`** (Depends on `build-and-test` & `security-audit`):
     * Executes exclusively on `main` and `develop` branches.
     * Compiles and publishes lightweight `Nms.Api` binaries (`dotnet publish -c Release -o ./publish-output /p:UseAppHost=false`).
     * Uploads versioned release artifacts (`nms-enterprise-api-release-<SHA>`) with a 14-day retention cycle.
  4. **`auto-merge-to-develop`** (Depends on `build-and-test` & `security-audit`):
     * Executes on verified `feature/**` branches to merge completed feature commits safely into `develop`.

---

### B. Continuous Delivery & Release Pipeline (`.github/workflows/release.yml`)
* **Trigger Conditions**:
  * Git tag pushes matching semantic versioning syntax (`v*.*.*`).
  * Manual execution via `workflow_dispatch` with parameter input `release_version`.
* **Workflow Stages**:
  1. **`validate-and-build`**:
     * Restores, builds, and executes full regression tests in Release mode.
     * Publishes standalone production binaries to `./release-package`.
     * Compiles and tags the production container image (`nms-enterprise-api:vX.Y.Z`).
     * Uploads immutable release packages (`nms-enterprise-api-<TAG>`) retained for 30 days.
  2. **`deploy-staging`** (Depends on `validate-and-build`):
     * Targets the `staging` environment for non-blocking smoke testing and automated validation.
  3. **`deploy-production`** (Depends on `deploy-staging`):
     * Targets the protected `production` environment.
     * Enforces repository environment protection rules, blocking execution until authorized manual approval is recorded.

---

## 3. GitHub Environments & Protection Gates

| Environment | Purpose | Protection Rule | Trigger Source |
| :--- | :--- | :--- | :--- |
| **`staging`** | Pre-production validation | Automated deployment (No wait timer, no blocking reviewers) | Any release tag / `develop` |
| **`production`** | Live enterprise operations | Required Reviewers (Manual sign-off required) | Official release tags (`v*.*.*`) / `main` |

### Setting Up Environment Rules in GitHub:
1. Go to repository **Settings** -> **Environments**.
2. Select or create the **`staging`** environment:
   * Keep *Required reviewers* unchecked.
3. Select or create the **`production`** environment:
   * Check **Required reviewers** and assign authorized repository maintainers.
   * Under *Deployment branches and tags*, select **Selected branches and tags** and add tag rule `v*.*.*`.

---

## 4. Pipeline Execution & Release Procedure

### Standard Feature Flow
1. Create branch `feature/<name>` from latest `develop`.
2. Commit changes and push to origin.
3. CI runs format checks, analyzer-enforced builds, Redis-backed integration tests, container builds, and security scans.
4. Upon passing all checks, `auto-merge-to-develop` integrates the branch into `develop`.

### Production Tagging & Release Flow
1. Create a lightweight annotated Git tag from `main`:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0