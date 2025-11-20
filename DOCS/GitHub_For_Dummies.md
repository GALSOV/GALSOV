# GitHub for Dummies
Date: 2025-11-18

---

## 1. What is GitHub?
GitHub is a platform for **version control** and **collaboration**. It uses **Git**, a system that tracks changes in files over time. Think of it as a time machine for your code.

### Why use GitHub?
- **Backup:** Your code is safe online.
- **Versioning:** Roll back to previous versions easily.
- **Collaboration:** Multiple people can work on the same project without overwriting each other.
- **Open Source:** Share your work or learn from others.

---

## 2. Key Concepts
- **Repository (Repo):** A folder for your project on GitHub.
- **Commit:** A snapshot of your changes.
- **Branch:** A separate line of development (e.g., `main` for stable, `dev` for new features).
- **Pull Request (PR):** A way to merge changes from one branch to another.

---

## 3. GitHub Website Basics
1. **Create an account** at [github.com](https://github.com).
2. Click **New Repository** → Name it → Choose Private or Public.
3. Add a **README.md** (explains your project).
4. Use **Issues** for tasks and **Projects** for roadmaps.

---

## 4. Local Setup
### Install Git
- Download from [git-scm.com](https://git-scm.com).
- Verify: `git --version` in Command Prompt.

### Clone a Repo
```bash
git clone https://github.com/<your-username>/<repo-name>.git
cd <repo-name>
```

### Common Commands
```bash
git status        # Check changes
git add .         # Stage all changes
git commit -m "Message"  # Save snapshot
git push origin main     # Upload to GitHub
git pull origin main     # Download latest changes
```

---

## 5. Branching Basics
```bash
git checkout -b feature/new-ui   # Create new branch
git push origin feature/new-ui   # Push branch to GitHub
```

---

## 6. Beyond Basics
- **.gitignore:** Tells Git which files to ignore (e.g., `bin/`, `obj/`).
- **Tags:** Mark versions (e.g., `v1.0`).
- **GitHub Actions:** Automate builds/tests.

---

## 7. Visual Tools
- **GitHub Desktop:** GUI for Git.
- **Visual Studio Code:** Great editor with Git integration.

