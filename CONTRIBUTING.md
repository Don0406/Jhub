Team Collaboration Guide (Option 1: Feature Branches & Pull Requests)

To keep our `main` branch clean and prevent code overwrites, **all team members must work on feature branches** and submit a **Pull Request (PR)** before merging changes into `main`.

---

## 🔄 Step-by-Step Developer Workflow

### Step 1: Sync Your Local `main` Branch
Before starting any new task or page conversion, always make sure your local `main` branch has the latest updates:

```bash
git checkout main
git pull origin main

Step 2: Create a New Feature Branch
Create and switch to a descriptive branch for the feature or view you are working on:

Bash
# Naming format: feature/page-name or fix/issue-name
git checkout -b feature/agency-dashboard

Step 3: Work Locally & Commit Your Changes
Make your code changes or convert your .html file to a .cshtml Razor view. Once tested and running locally, stage and commit your work:

Bash
git add .
git commit -m "Convert agency-dashboard.html to Views/Agency/Dashboard.cshtml"

Step 4: Push Your Branch to GitHub
Push your local branch up to the remote repository on GitHub:

Bash
git push -u origin feature/agency-dashboard

Step 5: Open a Pull Request (PR) on GitHub
Go to the repository on GitHub: https://github.com/Don0406/Jhub

You will see a banner near the top saying: "feature/agency-dashboard had recent pushes. Compare & pull request".

Click Compare & pull request.

Add a short summary of your changes (e.g., converted view, added controller action, updated CSS).

Click Create pull request.

Step 6: Code Review & Merging
Notify the team in group chat that your PR is ready.

Once reviewed, click Merge pull request on GitHub, then click Confirm merge.

You can safely click Delete branch on GitHub after merging.

Step 7: Clean Up Your Local Environment
After your PR is merged on GitHub, switch back to main locally and pull the newly merged code:

git checkout main
git pull origin main

# Optional: Delete your local feature branch since it's merged
git branch -d feature/agency-dashboard
