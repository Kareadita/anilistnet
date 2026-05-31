## 🧑‍💻 Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

### Prerequisites

- [.NET](https://dot.net) 10+

### Installation

1. Clone the repo: `git clone https://github.com/dentolos19/anilistnet.git`
2. Get your API key, learn how by [clicking here](https://github.com/dentolos19/anilistnet/wiki/Tutorials#authenticating-with-anilist).
3. Use the template `.env.template` and create a file named `.env` inside the project `AniListNet.Tests` and enter your key.
4. Restore dependencies: `dotnet restore` (optional)
5. Test the library: `dotnet test` or use the built-in tests runner in your IDE (recommended) (Tests are pending rewrite, they'll currently fail due to rate limits)

### Pull Requests

Pull requests should follow the template, the body is automaticlly used to generate the changelog.

### Publishing

To publish a version of the library to nuget, include the version string in your PR title. When merged, will release the new version
