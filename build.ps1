Push-Location .Rust
cargo build --target x86_64-pc-windows-msvc --all-features --release
Pop-Location

Copy-Item .Rust/target/x86_64-pc-windows-msvc/release/kataru_ffi.dll "Plugins/Windows/kataru_ffi.dll"
