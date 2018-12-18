<h1 align="center"><a href='https://github.com/true-myth/true-myth-csharp'>True Myth C♯</a></h1>

<p align="center">A library for safer programming in C♯, with <code>Maybe</code> and <code>Result</code> types, and supporting both a functional style and a more traditional method-call style.</p>

<p align="center">
  <a href='https://dev.azure.com/true-myth/TrueMyth/_build/latest?definitionId=1?branchName=master&view=results'>
    <img src='https://img.shields.io/azure-devops/build/true-myth/TrueMyth/1/master.svg?style=flat&logo=azuredevops' alt='Azure Pipelines `master` build status' />
  </a>
  <a href='https://dev.azure.com/true-myth/TrueMyth/_build/latest?definitionId=1?branchName=master&view=ms.vss-test-web.test-result-details'>
    <img src='https://img.shields.io/azure-devops/tests/true-myth/TrueMyth/1/master.svg?style=flat&logo=azuredevops' />
  </a>
  <a href='https://dev.azure.com/true-myth/TrueMyth/_build/latest?definitionId=1?branchName=master&view=results'>
    <img src='https://img.shields.io/azure-devops/coverage/true-myth/TrueMyth/1/master.svg?style=flag&logo=azuredevops' />
  </a>
  <a href='https://github.com/true-myth/true-myth/blob/master/LICENSE'>
    <img src='https://img.shields.io/github/license/true-myth/true-myth-csharp.svg?style=flat'>
  </a>
  <a href='https://www.nuget.org/packages/TrueMyth'>
    <img src='https://img.shields.io/nuget/dt/TrueMyth.svg?style=flat' alt='Hosted on Nuget.org' />
  </a>
</p>

## Overview

True Myth provides standard, type-safe wrappers and helper functions to help help you with two *extremely* common cases in programming:

-   not having a value
-   having a *result* where you need to deal with either success or failure

You could implement all of these yourself – it's not hard! – but it's much easier to just have one extremely well-tested library you can use everywhere to solve this problem once and for all.
