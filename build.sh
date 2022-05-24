# Build rust library
pushd .Rust
# Don't update for now, since this causes build errors.
# cargo update
cargo build --target x86_64-apple-darwin --all-features --release
popd

cp .Rust/target/x86_64-apple-darwin/release/libkataru_ffi.dylib "Plugins/OSX/kataru_ffi.dylib"
