# Release Process

This document describes how to create a new release of the NuGet package.

## Overview

Versioning for this project is based on [Semantic Versioning 2.0](https://semver.org/spec/v2.0.0.html) rules. GitHub
workflows automate much of the release process.

The release automation tooling relies on the [changelog](../CHANGELOG.md) to detect the current version number. The
changelog is assumed to follow [keep a changelog](https://keepachangelog.com/en/1.0.0/) conventions.

> **Note**
> The changelog parser can be a bit picky, and the error reporting is not too helpful, so ensure your headings
> are at the correct levels!

## Development workflow

![Workflow overview](./release_workflow.drawio.svg)

### 0. Make changes

This is a typical trunk-based workflow. You will probably repeat this several times between each release, as you fix
bugs and add features.

For each change:

- Create a branch (direct pushes to `main` are prohibited). Prefix the descriptive name with `feature/*` or `bugfix/*`
  to provide a hint as to the motivation for the change. If there's a related issue, it's handy to start the descriptive
  name with the issue number e.g. `feature/123-awesome-change`.

- Make your changes. Don't forget to update the `[Unreleased]` section of the changelog with a description of your
  changes!

- When your branch is ready to merge, create a pull request that targets the `main` branch. This triggers some CI
  workflows to validate the changes.

- Merge the PR onto `main`. Prefer `Rebase and merge` or `Squash and merge` over `Create a merge commit`, so that the
  commit history remains linear.

### 1. Initiate the release process

Release preparation starts by ensuring that the changelog is up to date. The tools assume that the changes have been
documented in the `[Unreleased]` section of the changelog, and the process will fail if the section is not present. This
is to ensure that each release is properly documented for consumers.

Once the `[Unreleased]` section of the changelog is up to date on the `main` branch, it's time to prepare a PR that
bumps the changelog version to signify a new release. This step is mostly automated - just run the `[autorelease]
Prepare release PR` workflow from the repo's Actions page. The parameters are:

- **Use workflow from:** `main`

- **Release type:** Select the appropriate value that defines how the release number will be incremented. See below for
  further details.

- **Pre-release identifier:** The pre-release prefix, e.g. `alpha`, `beta` etc.

This workflow creates a PR containing a commit that updates the changelog by modifying the `[Unreleased]` section title
to contain the incremented version number.

#### Release type notes

How to choose a release type? The documentation for the [semver
library](https://www.npmjs.com/package/semver#user-content-functions)'s `inc` function isn't extensive. Here are some
examples, I will write a full description later.

##### Examples

| Latest release   | release-type | prerelease-identifier | Result            |
| ---------------- | ------------ | --------------------- | ----------------- |
| `unreleased`     | prerelease   | `alpha`               | `0.0.1-alpha.0`   |
| `unreleased`     | preminor     | `alpha`               | `0.1.0-alpha.0`   |
| `0.1.0-alpha.1`  | prerelease   | `alpha`               | `0.1.0-alpha.2`   |
| `0.1.0-alpha.1`  | prerelease   | _empty_               | `0.1.0-alpha.2`   |
| `0.1.0-alpha.1`  | prerelease   | `beta`                | `0.1.0-beta.0`    |
| `0.1.0-beta.0`   | prerelease   | _empty_               | `0.1.0-beta.1`    |
| `0.1.0-beta.1`   | minor        | _empty_               | `0.1.0`           |
| `0.1.0-beta.1`   | preminor     | `alpha`               | `0.2.0-alpha.0`   |
| `0.1.0-beta.1`   | preminor     | `beta`                | `0.2.0-beta.0`    |
| `0.1.0-beta.1`   | minor        | `beta`                | `0.1.0`           |
| `0.1.0-beta.1`   | minor        | `alpha`               | `0.1.0`           |
| `0.1.0`          | prerelease   | `alpha`               | `0.1.1-alpha.0`   |

### 2. Merge the release PR

Once the PR is created, all you need to do is sanity check the contents of the PR, and then merge it back onto the
original branch. You should do this as soon as possible, because you don't want to include anything other than the code
that was on the original branch when you created the release PR. **Don't attempt to rebase subsequent changes from the
original branch onto the release PR**__ - this is not supported. If changes have been merged onto the original branch
since you created the release PR, you should abandon that PR and start over.

When you merge the PR back onto `main`, it triggers the release process. A workflow creates the release and attaches the
NuGet package as a release asset. The release is created in draft mode.

### 3. Publish the release to NuGet

All you need to do now is validate the draft release, then publish it. The draft release should contain the new
version's release notes from the changelog - just sanity check that everything looks ok - this is the last chance to
spot any howlers before the package is published.

When you publish the release, it triggers a workflow that automatically pushes the package to the NuGet feed. (Hint: to
publish the release in the GitHub UI, you need to edit it).

The package is now published! :rocket:
