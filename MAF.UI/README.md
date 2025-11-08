# MAF.UI - Blazor Frontend for Microsoft Agents Framework

A modern, dark-themed chat interface built with Blazor Server for the Microsoft Agents Framework.

## Features

### UI Components
- **Sidebar Navigation**: Contains New Chat button, chat history with timestamps, and settings button
- **Top Bar**: Shows app title, test mode indicator, and cloud/local mode toggle
- **Chat Interface**: 
  - Home screen with animated icon and conversation starters
  - Smooth transitions when starting a chat
  - Message bubbles with distinct styling for user vs AI messages
  - Typing indicator animation
  - Responsive design

### Mock Mode
The application includes a fully functional mock mode that:
- Simulates AI responses without requiring backend connectivity
- Pre-loads sample chat history for demonstration
- Enables UI development and testing independently

### Chat Features
- Create new chat sessions
- Switch between existing chats
- Conversation starter prompts
- Enter to send, Shift+Enter for new line
- Real-time message updates via SignalR

## Getting Started

### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code (optional)

### Running the Application

1. Navigate to the MAF.UI directory:
   ```bash
   cd MAF.UI
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Open your browser and navigate to:
   ```
   https://localhost:5150
   ```
   or
   ```
   http://localhost:5200
   ```

### Building from Solution

From the solution root:
```bash
cd MAF.Assistants.Runners
dotnet build MAF.Assistants.sln
dotnet run --project ../MAF.UI/MAF.UI.csproj
```

## Project Structure

```
MAF.UI/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          # Main application layout
│   │   ├── NavMenu.razor             # Sidebar navigation component
│   │   └── *.razor.css               # Component-specific styles
│   └── Pages/
│       ├── Home.razor                # Main chat page
│       └── Error.razor               # Error page
├── Models/
│   ├── AppSettings.cs                # Application settings model
│   ├── ChatMessage.cs                # Chat message model
│   └── ChatSession.cs                # Chat session model
├── Services/
│   └── ChatService.cs                # Chat state management service
├── wwwroot/
│   ├── app.css                       # Global styles
│   └── lib/                          # Third-party libraries
└── Program.cs                        # Application entry point
```

## Configuration

### Mock Mode
Mock mode is enabled by default. To switch to live mode (once backend integration is complete), update the `AppSettings` in `ChatService`:

```csharp
_settings.IsMockMode = false;
```

### Cloud/Local Mode
The cloud/local mode toggle is currently visual only. Future implementation will integrate with MAF.Assistants backend services.

## Development

### Adding New Features

1. **New Services**: Add service classes to `Services/` directory
2. **New Components**: Add Razor components to `Components/` directory
3. **New Models**: Add model classes to `Models/` directory

### Styling

The application uses custom CSS with a dark theme. Component-specific styles are in `*.razor.css` files, while global styles are in `wwwroot/app.css`.

## Future Enhancements

- [ ] Integrate with MAF.Assistants backend for real AI responses
- [ ] Implement actual file upload functionality
- [ ] Add settings page with configuration options
- [ ] Implement cloud/local mode switching logic
- [ ] Add persistent storage for chat history
- [ ] Add authentication and user management
- [ ] Add markdown rendering for AI responses
- [ ] Add code syntax highlighting
- [ ] Add export chat functionality

## Technology Stack

- **Framework**: Blazor Server (.NET 9.0)
- **Architecture**: Service-based with singleton ChatService
- **Real-time**: SignalR for live updates
- **Styling**: Custom CSS with dark theme
- **Interactivity**: Server-side rendering

## Screenshots

### Home Screen
The home screen displays conversation starter prompts to help users get started:

![Home Screen](https://github.com/user-attachments/assets/d7a865e5-1dcb-4503-9832-2ffb3bf8a3ef)

### Active Chat
Once a conversation is started, messages appear with distinct styling for user and AI:

![Active Chat](https://github.com/user-attachments/assets/a4d850ed-d405-4d78-af80-1d313962c947)

## Contributing

When contributing to this project:
1. Follow the existing code structure and patterns
2. Add appropriate comments for complex logic
3. Test thoroughly in both mock and live modes (when available)
4. Update this README if adding significant features

## License

See the main repository LICENSE file for details.
