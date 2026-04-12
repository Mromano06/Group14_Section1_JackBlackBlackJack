<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a id="readme-top"></a>
<!-- PROJECT SHIELDS -->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack">
    <img width="640" height="360" alt="Blackjack_logo" src="https://github.com/user-attachments/assets/b2a1a8dc-ed33-4570-9e70-cabdb919e6e9" />
  </a>

<h3 align="center">JackBlack BlackJack</h3>

  <p align="center">
    A multiplayer client-server blackjack game built with .NET 10.0, featuring custom binary protocol and action-based game logic
    <br />
    <a href="https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack">View Demo</a>
    &middot;
    <a href="https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    &middot;
    <a href="https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
        <li><a href="#architecture">Architecture</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#features">Features</a></li>
    <li><a href="#project-structure">Project Structure</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

JackBlack BlackJack is a work-in-progress multiplayer blackjack game built as part of a computer science course project. The game features a client-server architecture with a custom binary communication protocol, action-based game logic, and comprehensive unit testing.

**Key Highlights:**
* **Client-Server Architecture** - TCP-based networking with asynchronous send/receive loops
* **Custom Binary Protocol** - Efficient packet serialization using Jables_Protocol
* **Action Pattern** - Clean, testable game logic with executable actions (Bet, Hit, Stand, Double, Insure)
* **WPF Client** -  Desktop UI with card animations and visual feedback
* **Comprehensive Testing** - Unit tests for all game actions and logic components
* **Scalable Design** - Built to support single-player and future multiplayer sessions

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

* [![.NET][.NET-badge]][.NET-url]
* [![C#][CSharp-badge]][CSharp-url]
* [![WPF][WPF-badge]][WPF-url]
* [![TCP/IP][TCP-badge]][TCP-url]
* [![MSTest][MSTest-badge]][MSTest-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Architecture

The project follows a layered architecture pattern with clear separation between client and server:

**Client Side:**
- **WPF Client (UI)** - Desktop application with rich user interface
- **Jables_Protocol (Network)** - Custom binary serialization for communication
- **GameLogic (Business Logic)** - Action-based game rules and logic
- **SharedModels (Data Models)** - Card, Hand, Player, Dealer models

**Server Side:**
- **TCP Server** - Listens for client connections on port 27000
- **GameManager** - Orchestrates game flow and handles client messages
- **GameLogic (Business Logic)** - Server-authoritative game validation
- **SharedModels (Data Models)** - Shared data structures

**Communication:**
- Custom binary protocol (Jables_Protocol) over TCP
- PacketType-based message routing (PlayerCommand, GameState, Error)
- Asynchronous send/receive loops for real-time gameplay

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

Follow these steps to get the project running locally.

### Prerequisites

* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* Visual Studio 2022 or later (recommended) or VS Code with C# extension
* Windows OS (for WPF client)

### Installation

1. Clone the repository
```sh
   git clone https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack.git
```

2. Navigate to the project directory
```sh
   cd Group14_Section1_JackBlackBlackJack
```

3. Restore NuGet packages
```sh
   dotnet restore
```

4. Build the solution
```sh
   dotnet build
```

5. Run the tests (optional)
```sh
   dotnet test
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Playing the Game

1. **Place a Bet** - Enter your bet amount (minimum $5) and click "Bet"
2. **Receive Cards** - You'll get 2 cards, dealer shows 1 card face up
3. **Make Decisions**:
   - **Hit** - Draw another card
   - **Stand** - Keep your current hand
   - **Double** - Double your bet, draw one card, then stand (only available on first 2 cards)
   - **Insurance** - Take insurance if dealer shows an Ace (costs half your bet)
4. **View Results** - After you stand or bust, dealer plays and results are calculated
5. **Play Again** - Place a new bet to start the next round

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- FEATURES -->
## Features

### Core Gameplay
- [x] Standard blackjack rules (hit, stand, double, insurance)
- [x] Dealer AI that follows house rules (hits on 16, stands on 17)
- [x] Proper blackjack payouts (3:2 for blackjack, 2:1 for insurance)
- [x] Multi-deck shoe (configurable deck count)
- [x] Bust detection and automatic hand resolution

### Technical Features
- [x] Custom binary serialization protocol (Jables_Protocol)
- [x] Action-based game architecture (Command pattern)
- [x] Async TCP networking with concurrent send/receive loops
- [x] Comprehensive unit test coverage (>80%)
- [x] Server-authoritative game state (prevents cheating)
- [x] Error handling and validation at all layers

### Client Features
- [x] WPF desktop application with rich UI
- [x] Real-time game state updates
- [x] Visual card representations
- [x] Balance and bet tracking
- [x] Action button enable/disable based on game state

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ROADMAP -->
## Roadmap

### Sprint 1-5 (Completed)
- [x] Core game architecture
- [x] Action system implementation
- [x] Basic networking infrastructure
- [x] Unit test framework
- [x] Custom binary protocol (Jables_Protocol)
- [x] Server game loop with GameManager
- [x] WPF client UI
- [x] Base action implementations (Bet, Hit, Stand, Double)
- [x] Comprehensive unit testing

### Future Enhancements
- [ ] Multi-player support (GameSession managing multiple clients)
- [ ] Split action for pair hands
- [ ] Insure for dealer blackjacks
- [ ] Persistent user accounts and statistics
- [ ] Enhanced UI with animations
- [ ] Sound effects and music
- [ ] Configurable house rules

See the [open issues](https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/issues) for a full list of proposed features and known issues.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".

Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines

- Follow the existing code structure and naming conventions
- Write unit tests for new game logic
- Ensure all tests pass before submitting PR
- Update documentation for new features
- Keep commits atomic and well-described

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Top contributors:

<a href="https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Mromano06/Group14_Section1_JackBlackBlackJack" alt="contrib.rocks image" />
</a>

<!-- LICENSE -->
## License

TBD

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Project Link: [https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack](https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [Best-README-Template](https://github.com/othneildrew/Best-README-Template)
* [Shields.io](https://shields.io)
* [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
* [WPF Tutorial](https://wpf-tutorial.com/)
* Course Instructors and TAs for guidance and support

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/Mromano06/Group14_Section1_JackBlackBlackJack.svg?style=for-the-badge
[contributors-url]: https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Mromano06/Group14_Section1_JackBlackBlackJack.svg?style=for-the-badge
[forks-url]: https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/network/members
[stars-shield]: https://img.shields.io/github/stars/Mromano06/Group14_Section1_JackBlackBlackJack.svg?style=for-the-badge
[stars-url]: https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/stargazers
[issues-shield]: https://img.shields.io/github/issues/Mromano06/Group14_Section1_JackBlackBlackJack.svg?style=for-the-badge
[issues-url]: https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/issues
[license-shield]: https://img.shields.io/github/license/Mromano06/Group14_Section1_JackBlackBlackJack.svg?style=for-the-badge
[license-url]: https://github.com/Mromano06/Group14_Section1_JackBlackBlackJack/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/linkedin_username
[product-screenshot]: images/screenshot.png

[.NET-badge]: https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[.NET-url]: https://dotnet.microsoft.com
[CSharp-badge]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white
[CSharp-url]: https://docs.microsoft.com/en-us/dotnet/csharp/
[WPF-badge]: https://img.shields.io/badge/WPF-512BD4?style=for-the-badge&logo=windows&logoColor=white
[WPF-url]: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/
[TCP-badge]: https://img.shields.io/badge/TCP%2FIP-0078D4?style=for-the-badge&logo=cisco&logoColor=white
[TCP-url]: https://en.wikipedia.org/wiki/Transmission_Control_Protocol
[MSTest-badge]: https://img.shields.io/badge/MSTest-512BD4?style=for-the-badge&logo=visualstudio&logoColor=white
[MSTest-url]: https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest
