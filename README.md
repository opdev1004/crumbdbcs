# 🥇 CrumbDB CS
NoSQL Document DBMS in C#. Crumbdb CS is a minimalistic JSON database library that stores documents as individual JSON files. It supports safe concurrent access using async file locking and includes optional backup functionality via ZIP compression. CrumbDB is designed and built for solving a problem with data file size limits.

## 👨‍🏫 Notice

### 🎉 Releasing version 0.7.0
PLEASE USE THE LATEST VERSION.
- Upgraded `getMultiple()`, now it returns `MultipleData` Class.
- Added `getMultipleByKeyword()` and `getMultipleByKeywords()`
- Functions that requires only full directory path are removed.
- Fixed algorithm for `getMultiple`, `getMultipleByKeyword()` and `getMultipleByKeywords()`

### 📢 Note
- Support dotNet 8 and 9. Dotnet 8 is going to be supported until it is no longer compatible with the newest version of dotnet.

## 📖 Documents
None for now

## 🛠 Requirements

CrumbDB CS is built for .net 8.0 and .net 9.0 and Windows 10. I cannot guarantee that this will work in older versions of Windows or other OS and with other tools.

## 💪 Support CrumbDB CS

### 👼 Become a Sponsor

- [Github sponsor page](https://github.com/sponsors/opdev1004)

## 👨‍💻 Author

[Victor Chanil Park](https://github.com/opdev1004)

## 💯 License

MIT, See [LICENSE](./LICENSE).
