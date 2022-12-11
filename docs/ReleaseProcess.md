# Release Process

This document describes how to create a new release of the NuGet package.

## Releasing a new version

Versioning for these actions is based on GitHub's [action
versioning](https://github.com/actions/toolkit/blob/master/docs/action-versioning.md)
recommendations, with some workflows that automate much of the process.

The release automation tooling relies on the [changelog](../CHANGELOG.md) to detect the current
version number. The changelog is assumed to follow [keep a
changelog](https://keepachangelog.com/en/1.0.0/) conventions.

### Update the changelog

Release preparation starts by ensuring that the changelog is up to date. The tools assume that the
changes have been documented in the `Unreleased` section of the changelog, and the process will fail
if the section is not present. This is to ensure that each release is properly documented for
consumers.

### Preparing a release PR

Once the `Unreleased` section of the changelog is up to date, it's time to prepare a PR that bumps the changelog
version. This step is mostly automated - all you need to do is to run the `[autorelease] Prepare release PR` workflow
from the GitHub Actions page. This workflow generates a PR that updates the changelog with the next version number. When
running the action, use the `Release type` input to select what kind of version bump to perform, e.g. major, minor,
patch, etc. - see below.

Typically, you will run this action on the `main` branch, as releases are taken from there.

#### Release type notes

How to choose a release type? The documentation for the semver `inc` function isn't extensive. Here are some notes, I
will write a full description later.

##### Examples

| Latest release | Release type        | Result             |
| -------------- | --------------------| ------------------ |
| 0.1.0-alpha.0  | prerelease (alpha)  | 0.1.0-alpha.1      |
| 0.1.0-alpha.2  | minor               | 0.1.0              |

### Validate and merge the release PR

Once the PR is created, all you need to do is sanity check the contents of the PR, and then merge it
back onto the original branch. You should do this as soon as possible, because you don't want to
include anything other than the code that was on the original branch when you created the release
PR. **Don't attempt to rebase subsequent changes from the original branch onto the release PR** -
this is not supported. If changes have been merged onto the original branch since you created the
release PR, you should abandon that PR and start over.

### Publish the release

When the release PR is merged, it triggers an automated workflow that creates a draft GitHub
release. All you need to do now is validate the release, then publish it.

Your action is now published! :rocket:

At this point, automation will create or update the major version tag, e.g. `v1`. The process is
described in the action versioning
[documentation](https://github.com/actions/toolkit/blob/master/docs/action-versioning.md).
