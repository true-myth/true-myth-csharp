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

### Contents

- [Setup](#setup)
- [Just the API, please](#just-the-api-please)
    - [`Result` with a functional style](#result-with-a-functional-style)
    - [`Maybe` with the method style](#maybe-with-the-method-style)
    - [Constructing `Maybe`](#constructing-maybe)
    - [Safely getting at values](#safely-getting-at-values)
    - [Curried variants](#curried-variants)
- [Why do I need this?](#why-do-i-need-this)
    - [1. Nothingness: `null` and `undefined`](#1-nothingness-null-and-undefined)
    - [2. Failure handling: callbacks and exceptions](#2-failure-handling-callbacks-and-exceptions)
- [Solutions: `Maybe` and `Result`](#solutions-maybe-and-result)
    - [How it works: `Maybe`](#how-it-works-maybe)
    - [How it works: `Result`](#how-it-works-result)
- [Design philosophy](#design-philosophy)
    - [A note on reference types: no deep copies here!](#a-note-on-reference-types-no-deep-copies-here)
    - [The type names](#the-type-names)
        - [`Maybe`](#maybe)
            - [The `Maybe` variants: `Just` and `Nothing`](#the-maybe-variants-just-and-nothing)
        - [`Result`](#result)
            - [The `Result` variants: `Ok` and `Err`](#the-result-variants-ok-and-err)
    - [Inspiration](#inspiration)
- [What's with the name?](#whats-with-the-name)
