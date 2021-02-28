# Build rust library
pushd .Rust
cargo build --target x86_64-apple-darwin --all-features --release
popd

cp .Rust/target/x86_64-pc-windows-msvc/release/kataru_ffi.dylib "Plugins/OSX/kataru_ffi.dylib"
