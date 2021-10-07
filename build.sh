# Build rust library
pushd .Rust
cargo update
cargo build --target x86_64-apple-darwin --all-features --release
cargo build --target x86_64-unknown-linux-gnu --all-features --release
popd

cp .Rust/target/x86_64-apple-darwin/release/libkataru_ffi.dylib "Plugins/OSX/kataru_ffi.dylib"
cp .Rust/target/x86_64-unknown-linux-gnu/release/libkataru_ffi.so "Plugins/Linux/kataru_ffi.so"
