# 📝 Git Commit Message Guidelines

> *Well-structured commit messages create a meaningful project history that helps future developers understand your work.*

## 🔍 Core Principles

### **Use all lowercase**
- Write the entire commit message in lowercase letters
- ✅ `add login feature` 
- ❌ `Add Login Feature`

  ### **Write in imperative mood**
  - Start descriptions with action verbs in command form
  - Think: "This commit will..." + your description
  - ✅ `🔒✨ feat(auth): add login feature` - "add" is imperative
  - ✅ `🐛 fix(ui): correct display bug` - "correct" is imperative
  - ❌ Avoid past tense: "added", "fixed", "implemented"

  ### **Keep subject under 50 characters**
  - Make the first line concise and scannable
  - Prioritize clarity over completeness
  - ✅ `✨ feat(auth): add login feature` *(30 chars)*

  ### **Separate subject from body**
  - Always include a blank line between subject and body
  - This spacing is required for Git tools to properly parse the message
  - Improves readability in logs and UIs

  ### **Explain what and why in the body**
  - Focus on **why** the change was made, not just what was done
  - Address the problem being solved and the motivation
  - Keep it concise but informative - no strict length limit
  - Optional for trivial changes, recommended for most commits

  **Example:**
  ```
  implemented oauth2 authentication to replace the legacy system. 
  this enhances security and provides foundation for future 
  sso integrations across our services.
  ```

  ### **Use bullet points for multiple changes**
  - List distinct changes after the main body paragraph
  - Use hyphens (`-`) or asterisks (`*`)
  - Keep each point focused on a single aspect
  - Start each point with an action verb

  **Example:**
  ```
  implemented oauth2 authentication flow. improves security for user data.

  - add login form with validation
  - implement oauth2 token handling
  - add user session management
  - create secure storage for credentials
  ```

  ## 📋 Conventional Commits Format

  ### **Basic Structure**
  ```
  <emoji> <type>(<scope>): <description>

  <body>

  <footer>

  ### <issue references>
  ```

  ### **Type and Scope**
  - **Type**: Must be lowercase (`feat`, `fix`, `docs`, `style`, `refactor`, etc.)
  - **Scope** (optional): Area affected in parentheses (`auth`, `ui`, `api`)
  - **Description**: Short summary starting with imperative verb

  ### **Breaking Changes**
  - Add `!` after type/scope: `feat(auth)!: replace login system`
  - AND/OR include a footer: `BREAKING CHANGE: description of breaking change`
  - Breaking change footer must appear before the final `###` line

  ### **Examples**
  - Standard feature: `🔒✨ feat(auth): add login feature`
  - Breaking change: `💥 refactor(core)!: restructure user module`

  ### **Use Gitmoji**
  - Add a descriptive emoji at the beginning of the subject line
  - For features, choose an emoji that represents the feature type
  - You can combine specific emojis with ✨ (e.g., `🔒✨`)
  - Enhances visual scanning of commit history

  **Examples:**
  - Security feature: `🔒 feat(auth): implement password reset`
  - Shopping feature: `🛒✨ feat(shop): add cart functionality`
  - Bug fix: `🐛 fix(ui): resolve button alignment`

## 🎨 Emoji Reference Guide

### **Feature-Specific Emojis**
| Feature Area | Emoji | Example |
|-------------|-------|--------|
| Authentication | 🔒 | `🔒✨ feat(auth): add 2fa setup` |
| E-commerce | 🛒 | `🛒✨ feat(shop): add cart functionality` |
| Search | 🔍 | `🔍✨ feat(search): implement filters` |
| Notifications | 🔔 | `🔔✨ feat(alerts): add push notifications` |
| Analytics | 📊 | `📊✨ feat(stats): add user metrics dashboard` |
| UI/UX | 🎨 | `🎨✨ feat(ui): add new theme selector` |
| Configuration | ⚙️ | `⚙️✨ feat(config): add user preferences` |

### **Common Operation Emojis**
| Type | Emoji | Example | Purpose |
|------|-------|---------|--------|
| `fix` | 🐛 | `🐛 fix(api): correct validation logic` | Bug fixes |
| `docs` | 📝 | `📝 docs(readme): update setup guide` | Documentation |
| `style` | 🎨 | `🎨 style(css): reformat components` | Code formatting |
| `refactor` | ♻️ | `♻️ refactor(core): simplify auth flow` | Code restructuring |
| `perf` | ⚡ | `⚡ perf(db): optimize user queries` | Performance |
| `test` | ✅ | `✅ test(auth): add login validation tests` | Testing |
| `chore` | 🔧 | `🔧 chore(deps): update react version` | Maintenance |
| `ci` | 👷 | `👷 ci(github): add automated testing` | CI/CD |
| `build` | 📦 | `📦 build(webpack): optimize bundle size` | Build system |
| `revert` | ⏪ | `⏪ revert: "feat(user): add profile page"` | Reverting |

  ## 🔗 Issue Tracking References

  ### **Required `###` Line**
  - Every commit message **must** end with a line starting with `###`
  - This line appears after all other content including any `BREAKING CHANGE:` notes
  - Used primarily for linking to issue tracking systems

  ### **Reference Formats**
  - With issue reference: `### Closes #123` or `### Fixes #456, Refs #789`
  - Without issue reference: Use plain `###` on the final line

  **Examples:**
  ```
  # No issue reference with breaking change
  ...commit message body...

  BREAKING CHANGE: description of breaking change.

  ###
  ```

  ```
  # With issue reference
  ...commit message body...

  ### Closes #123
  ```

## Complete Example

### Good Example

```
🔒✨ feat(auth)!: implement new oauth2 login flow

replaced the legacy authentication system with a modern oauth2 flow. this significantly improves security and prepares for single sign-on capabilities. the new flow uses industry-standard protocols.

- add new login form with updated ui components
- implement oauth2 token exchange and secure validation
- migrate existing user sessions to the new system seamlessly
- update all relevant user documentation for the new login process

BREAKING CHANGE: the old `/auth/legacy-login` api endpoint has been removed. clients must update to use the new `/auth/oauth2/token` endpoint. user sessions from before this change will be invalidated, requiring re-authentication.

### Closes #123, Refs #456
```

### Bad Example

```
✨ feat: Added new login screen and fixed some bugs

This commit adds a new login screen with username and password fields. It also fixes several bugs related to form validation and improves the overall UI design with new styles.
###
```
