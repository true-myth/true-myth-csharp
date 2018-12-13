<h1 align="center"><a href='https://github.com/true-myth/true-myth-csharp'>True Myth C♯</a></h1>

<p align="center">A library for safer programming in C♯, with <code>Maybe</code> and <code>Result</code> types, and supporting both a functional style and a more traditional method-call style.</p>

<p align="center">
  <a href='https://ci.appveyor.com/project/aggieben/true-myth'>
    <img src='https://img.shields.io/appveyor/ci/aggieben/true-myth/next.svg?style=flat' alt='AppVeyor `next` build status'>
  </a>
  <a href='https://ci.appveyor.com/project/aggieben/true-myth/build/tests'>
    <img src='https://img.shields.io/appveyor/tests/aggieben/true-myth/next.svg?style=flat' alt='AppVeyor `next` test status'>
  </a>
  <a href='https://github.com/true-myth/true-myth/blob/master/LICENSE'>
    <img src='https://img.shields.io/github/license/true-myth/true-myth-csharp.svg?style=flat'>
  </a>
  <a href='https://www.nuget.org/packages/TrueMyth'>
    <img src='https://img.shields.io/nuget/dt/TrueMyth.svg?style=flat' alt='Hosted on Nuget.org'>
  </a>
</p>

## Overview

True Myth provides standard, type-safe wrappers and helper functions to help help you with two *extremely* common cases in programming:

-   not having a value
-   having a *result* where you need to deal with either success or failure

You could implement all of these yourself – it's not hard! – but it's much easier to just have one extremely well-tested library you can use everywhere to solve this problem once and for all.
