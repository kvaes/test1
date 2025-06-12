# Security Policy

## Supported Versions

We release patches for security vulnerabilities. Which versions are eligible for receiving such patches depends on the CVSS v3.0 Rating:

| CVSS v3.0 | Supported Versions                        |
| --------- | ----------------------------------------- |
| 9.0-10.0  | Releases within the last three months     |
| 4.0-8.9   | Most recent release                       |

## Reporting a Vulnerability

We take the security of our software seriously. If you discover a security vulnerability, please follow these steps:

### How to Report

1. **Do not** open a public GitHub issue for security vulnerabilities
2. Send an email to the repository owner at the email address associated with their GitHub account
3. Include the following information:
   - Type of issue (e.g. buffer overflow, SQL injection, cross-site scripting, etc.)
   - Full paths of source file(s) related to the manifestation of the issue
   - The location of the affected source code (tag/branch/commit or direct URL)
   - Any special configuration required to reproduce the issue
   - Step-by-step instructions to reproduce the issue
   - Proof-of-concept or exploit code (if possible)
   - Impact of the issue, including how an attacker might exploit the issue

### What to Expect

- You will receive a confirmation of your report within 48 hours
- We will provide a detailed response within 7 days indicating the next steps
- We will keep you informed of the progress towards a fix and full announcement
- We may ask for additional information or guidance

### Security Update Process

1. Security vulnerabilities are assessed and prioritized
2. Fixes are developed and tested
3. Security advisories are prepared
4. Patches are released with appropriate versioning
5. Security advisories are published

## Security Best Practices

When contributing to this project, please follow these security guidelines:

### Code Review
- All code changes require review before merging
- Security-sensitive changes require additional review
- Use static analysis tools to identify potential vulnerabilities

### Dependencies
- Keep dependencies up to date
- Monitor security advisories for used packages
- Use dependabot for automated dependency updates

### Authentication & Authorization
- Never commit secrets, API keys, or credentials to the repository
- Use environment variables or secure key management for sensitive data
- Implement proper input validation and sanitization

### Docker Security
- Use minimal base images
- Run containers as non-root users
- Regularly update base images
- Scan images for vulnerabilities

## Incident Response

In case of a security incident:

1. **Immediate Response**: Identify and contain the issue
2. **Assessment**: Evaluate the scope and impact
3. **Mitigation**: Implement temporary fixes if necessary
4. **Resolution**: Develop and deploy permanent fixes
5. **Communication**: Notify affected users and stakeholders
6. **Post-Incident**: Conduct review and improve processes

## Contact Information

For security-related questions or concerns, please contact the repository maintainer through the secure channels mentioned above.

## Acknowledgments

We appreciate the efforts of security researchers and the community in helping to keep our project secure. Contributors who responsibly disclose security vulnerabilities will be acknowledged in our security advisories (with their permission).