Push-Location .Rust
cargo update
cargo build --target x86_64-pc-windows-msvc --all-features --release
# cargo +nightly rustc --target x86_64-pc-windows-msvc --all-features -Z build-std -Z unstable-options --crate-type=staticlib --release -- --emit=llvm-bc
Pop-Location

Copy-Item .Rust/target/x86_64-pc-windows-msvc/release/kataru_ffi.dll "Plugins/Windows/kataru_ffi.dll"
# emar r .Rust/target/release/deps/kataru_ffi.a .Rust\target\x86_64-pc-windows-msvc\release\deps\kataru_ffi-*.bc
# Copy-Item .Rust/target/release/deps/kataru_ffi.a "Plugins/WebGL/kataru_ffi.bc"
