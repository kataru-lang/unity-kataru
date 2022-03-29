<img src="https://kataru-lang.github.io/_media/logo.svg" alt="Yarn Spinner logo" width="100px;" align="left">

# Kataru 「カタル」 The minimal dialogue system written in Rust. Now ported to Unity.

![rust workflow](https://github.com/katsutoshii/kataru/actions/workflows/rust.yml/badge.svg)

[Kataru 「カタル」](https://github.com/kataru-lang/kataru) is a system for interactive dialogue based on YAML, parsed in Rust. 

Unity-Kataru packages it nicely with scripts that make it possible to use in Unity.

Kataru is similar to [Twine](http://twinery.org/) and [Yarn Spinner](http://yarnspinner.dev) except with more support for organizing passages and sharing common functionality across multiple characters.

```yml
---
# Define what namespace this file is in.
namespace: global

state: 
  coffee: 0
  $passage.completed: 0

# Configure the scene. List your characters, commands, etc.
characters:
  May:
  June: 

commands:
  Wait:
    duration: 0.3

  $character.SetAnimatorTrigger:
    clip: ""

onExit:
  set:
    $passage.completed +: 1
---

Start:
  - May: Welcome to my story!
  - June: Want a coffee?
  - choices:
      Yes: YesCoffee
      No: NoCoffee

YesCoffee:
  - May: Yeah, thanks!
  - set:
        $coffee +: 1
  - May.SetAnimatorTrigger: [ "drinkcoffee" ]
  - call: End

NoCoffee:
  - May: No thanks.
  - Wait: { duration: 1 }
  - June: Want to end this story?
  - call: End

End:
  - May: The end!
```

## Features
- Simple and lightweight
- Organize dialogue, state, characters, and commands into local namespaces
- Character-specific commands
- Syntax highlighting and Unity integration
  
As well as conditionals, variables, and everything else you expect in a dialogue language.

## Getting Started

Read [kataru-lang.github.io/#/installation](https://kataru-lang.github.io/#/installation).

Once downloaded, check out the [examples](https://kataru-lang.github.io/#/examples).

## Getting Help

For Unity-Kataru bugs or feature requests, file an issue. Kataru bugs should get filed in the [kataru](https://github.com/kataru-lang/kataru) repo. For other concerns, contact kataru-dev@gmail.com.

# Notes on WebGL

To produce a valid LLVM bytecode file for Unity to consume, we use `build-std` to compile the standard library into the produced bytecode.
This is done via:

```
rustup component add rust-src --toolchain nightly
cargo +nightly rustc --target x86_64-pc-windows-msvc --all-features -Z build-std -Z unstable-options --crate-type=staticlib --release -- --emit=llvm-bc
emar r .Rust/target/release/deps/kataru_ffi.a .Rust\target\x86_64-pc-windows-msvc\release\deps\kataru_ffi-*.bc
```

## License

Unity-Kataru is licensed under the [MIT License](LICENSE). Credit is appreciated but not required.

---

Made by [Josiah Putman](https://github.com/Katsutoshii) with help from [Angela He](https://github.com/zephyo).

