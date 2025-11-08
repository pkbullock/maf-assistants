# Adaptive Cards Integration

This document describes the Adaptive Cards integration in the MAF.UI project, which enables rich, interactive card-based responses in the chat interface.

## Overview

Adaptive Cards are platform-agnostic snippets of UI that can be delivered to apps and websites. They provide a consistent, interactive experience for displaying structured information.

Reference: https://adaptivecards.microsoft.com/

## Features

The MAF.UI Adaptive Cards implementation includes:

- **Card Rendering**: Server-side rendering of Adaptive Card JSON to HTML
- **Multiple Card Types**: Support for various card elements and layouts
- **Styled Display**: Dark-themed styling consistent with the MAF.UI design
- **Mock Mode Support**: Sample cards for development and testing
- **Flexible Integration**: Easy to add cards to AI responses

## Architecture

### Components

1. **AdaptiveCardService** (`Services/AdaptiveCardService.cs`)
   - Parses and renders Adaptive Card JSON to HTML
   - Supports templating with data binding
   - Validates card structure
   - Handles various card elements

2. **ChatMessage Model Extension** (`Models/ChatMessage.cs`)
   - Added `AdaptiveCardJson` property for card content
   - Added `HasAdaptiveCard` property for easy checking

3. **ChatService Updates** (`Services/ChatService.cs`)
   - Integrated AdaptiveCardService
   - Added mock card generation for testing
   - Automatic card rendering in responses

4. **UI Integration** (`Components/Pages/Home.razor`)
   - Renders adaptive cards within chat messages
   - Seamless integration with existing message display

5. **Styling** (`wwwroot/app.css`)
   - Dark-themed card styling
   - Responsive design
   - Support for all card elements

## Supported Card Elements

The implementation supports the following Adaptive Card elements:

- **TextBlock**: Text content with various sizes, weights, and colors
- **Image**: Images with size controls and alt text
- **Container**: Grouping of elements
- **ColumnSet**: Multi-column layouts
- **FactSet**: Key-value pair display
- **ImageSet**: Gallery of images

### Supported Actions

- **OpenUrl**: Opens a URL in a new tab
- **Submit**: Submit button (displays alert in current implementation)
- **ShowCard**: Show card action (displays button)

## Usage

### Basic Usage in Mock Mode

The mock mode automatically generates adaptive cards when users mention certain keywords:

```
User: "Show me the weather"
→ Displays a weather card with temperature, conditions, and details

User: "What's the project status?"
→ Displays a project status card with facts and actions

User: "Show my profile"
→ Displays a profile card with information
```

### Programmatic Usage

To include an adaptive card in an AI response:

```csharp
var response = new ChatMessage
{
    Content = "Here's the information you requested:",
    AdaptiveCardJson = @"{
        ""type"": ""AdaptiveCard"",
        ""version"": ""1.5"",
        ""body"": [
            {
                ""type"": ""TextBlock"",
                ""text"": ""Hello World"",
                ""size"": ""Large""
            }
        ]
    }",
    IsUser = false
};
```

### Custom Card Examples

#### Weather Card

```json
{
    "type": "AdaptiveCard",
    "version": "1.5",
    "body": [
        {
            "type": "TextBlock",
            "text": "Seattle Weather",
            "size": "Large",
            "weight": "Bolder"
        },
        {
            "type": "ColumnSet",
            "columns": [
                {
                    "type": "Column",
                    "width": "auto",
                    "items": [
                        {
                            "type": "Image",
                            "url": "https://adaptivecards.io/content/weather-sunny.png",
                            "size": "Small"
                        }
                    ]
                },
                {
                    "type": "Column",
                    "width": "stretch",
                    "items": [
                        {
                            "type": "TextBlock",
                            "text": "72°F",
                            "size": "ExtraLarge"
                        }
                    ]
                }
            ]
        }
    ]
}
```

#### Status Card

```json
{
    "type": "AdaptiveCard",
    "version": "1.5",
    "body": [
        {
            "type": "TextBlock",
            "text": "Project Status",
            "size": "Large",
            "weight": "Bolder"
        },
        {
            "type": "FactSet",
            "facts": [
                {
                    "title": "Status",
                    "value": "In Progress"
                },
                {
                    "title": "Completion",
                    "value": "75%"
                }
            ]
        }
    ],
    "actions": [
        {
            "type": "Action.OpenUrl",
            "title": "View Details",
            "url": "https://example.com"
        }
    ]
}
```

## Styling and Theming

The adaptive cards are styled to match the MAF.UI dark theme:

- **Background**: Dark semi-transparent
- **Text Colors**: Light gray with accent colors
- **Borders**: Subtle borders with rounded corners
- **Actions**: Styled as buttons with hover effects
- **Facts**: Key-value pairs with clear hierarchy

All styles are defined in `wwwroot/app.css` under the "Adaptive Card Styles" section.

## Integration with AI Agents

When integrating with real AI agents (non-mock mode), the agent can return adaptive card JSON in the response. The ChatService will automatically detect and render these cards.

Example agent response format:

```json
{
    "content": "Here's your weather forecast:",
    "adaptiveCard": {
        "type": "AdaptiveCard",
        "version": "1.5",
        "body": [...]
    }
}
```

## Mock Mode

In mock mode, the system provides sample adaptive cards for development and testing:

1. **Weather Card**: Triggered by keywords: "weather", "forecast"
2. **Status Card**: Triggered by keywords: "status", "project"
3. **Profile Card**: Triggered by keywords: "profile"
4. **Default Card**: Triggered by keyword: "card", or when requesting "adaptive card"

## Testing

To test adaptive cards:

1. Run the MAF.UI application
2. Enable Mock Mode (default)
3. Send messages containing trigger keywords:
   - "show me the weather"
   - "what's the project status?"
   - "show my profile"
   - "show me an adaptive card"

## Development

### Adding New Card Types

1. Create the card JSON structure
2. Add it to `GenerateMockAdaptiveCard` method in `ChatService.cs`
3. Test the rendering in the UI

### Extending Card Elements

To add support for new Adaptive Card elements:

1. Add a new render method in `AdaptiveCardService.cs`
2. Add the case to the `RenderElement` switch statement
3. Add corresponding CSS styles in `app.css`

### Customizing Styles

All adaptive card styles are in `wwwroot/app.css` under the "Adaptive Card Styles" section. Modify these to customize the appearance.

## Best Practices

1. **Keep Cards Simple**: Focus on essential information
2. **Use Appropriate Sizes**: Choose text sizes that create clear hierarchy
3. **Provide Alt Text**: Always include alt text for images
4. **Test Rendering**: Verify cards render correctly in the UI
5. **Handle Errors**: The service gracefully handles invalid card JSON
6. **Use Templates**: For dynamic content, use Adaptive Card templating

## Limitations

- Submit actions display alerts (not implemented for form submission)
- ShowCard actions are not fully interactive
- Input elements are not currently supported
- Some advanced card features may not be rendered

## Future Enhancements

- [ ] Full support for input elements (TextInput, DateInput, etc.)
- [ ] Interactive ShowCard actions
- [ ] Form submission handling for Submit actions
- [ ] Card action callbacks
- [ ] Adaptive Card Designer integration
- [ ] Template library for common card types
- [ ] Accessibility improvements

## Resources

- [Adaptive Cards Documentation](https://adaptivecards.io/)
- [Adaptive Card Designer](https://adaptivecards.io/designer/)
- [Adaptive Cards Samples](https://adaptivecards.io/samples/)
- [Card Schema Explorer](https://adaptivecards.io/explorer/)

## Troubleshooting

### Card Not Rendering

- Verify the JSON is valid Adaptive Card schema
- Check browser console for JavaScript errors
- Ensure `AdaptiveCardJson` property is set on the message

### Styling Issues

- Check that `wwwroot/app.css` includes adaptive card styles
- Verify CSS classes match the service output
- Clear browser cache if styles don't update

### Mock Cards Not Appearing

- Verify Mock Mode is enabled in settings
- Check that message contains trigger keywords
- Review `GenerateMockAdaptiveCard` method logic

## Contributing

When adding new features or fixing bugs:

1. Update the `AdaptiveCardService` as needed
2. Add or modify mock cards in `ChatService`
3. Update CSS styles for any new elements
4. Update this documentation
5. Test thoroughly in mock mode

## License

See the main repository LICENSE file for details.
