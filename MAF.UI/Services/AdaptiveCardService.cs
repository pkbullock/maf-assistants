using AdaptiveCards;
using AdaptiveCards.Templating;
using Newtonsoft.Json;

namespace MAF.UI.Services;

public class AdaptiveCardService
{
    /// <summary>
    /// Renders an Adaptive Card JSON into HTML markup
    /// </summary>
    /// <param name="cardJson">The Adaptive Card JSON schema</param>
    /// <param name="dataJson">Optional data context for templating</param>
    /// <returns>HTML string representing the rendered card</returns>
    public string RenderCardToHtml(string cardJson, string? dataJson = null)
    {
        try
        {
            // If data is provided, apply templating
            if (!string.IsNullOrEmpty(dataJson))
            {
                var template = new AdaptiveCardTemplate(cardJson);
                cardJson = template.Expand(dataJson);
            }

            // Parse the card
            var parseResult = AdaptiveCard.FromJson(cardJson);
            
            if (parseResult.Card == null)
            {
                return $"<div class='adaptive-card-error'>Error: Unable to parse Adaptive Card</div>";
            }

            // Convert the card to HTML representation
            var html = ConvertCardToHtml(parseResult.Card);
            
            return html;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering adaptive card: {ex.Message}");
            return $"<div class='adaptive-card-error'>Error rendering card: {ex.Message}</div>";
        }
    }

    /// <summary>
    /// Validates if a string is a valid Adaptive Card JSON
    /// </summary>
    public bool IsValidAdaptiveCard(string json)
    {
        try
        {
            var card = JsonConvert.DeserializeObject<dynamic>(json);
            return card?.type == "AdaptiveCard";
        }
        catch
        {
            return false;
        }
    }

    private string ConvertCardToHtml(AdaptiveCard card)
    {
        var html = "<div class='adaptive-card'>";
        
        // Render card body
        if (card.Body != null && card.Body.Count > 0)
        {
            html += "<div class='adaptive-card-body'>";
            foreach (var element in card.Body)
            {
                html += RenderElement(element);
            }
            html += "</div>";
        }

        // Render card actions
        if (card.Actions != null && card.Actions.Count > 0)
        {
            html += "<div class='adaptive-card-actions'>";
            foreach (var action in card.Actions)
            {
                html += RenderAction(action);
            }
            html += "</div>";
        }

        html += "</div>";
        return html;
    }

    private string RenderElement(AdaptiveElement element)
    {
        return element switch
        {
            AdaptiveTextBlock textBlock => RenderTextBlock(textBlock),
            AdaptiveImage image => RenderImage(image),
            AdaptiveContainer container => RenderContainer(container),
            AdaptiveColumnSet columnSet => RenderColumnSet(columnSet),
            AdaptiveFactSet factSet => RenderFactSet(factSet),
            AdaptiveImageSet imageSet => RenderImageSet(imageSet),
            _ => $"<div class='adaptive-element-unsupported'>Unsupported element: {element.Type}</div>"
        };
    }

    private string RenderTextBlock(AdaptiveTextBlock textBlock)
    {
        var sizeClass = textBlock.Size switch
        {
            AdaptiveTextSize.Small => "text-small",
            AdaptiveTextSize.Medium => "text-medium",
            AdaptiveTextSize.Large => "text-large",
            AdaptiveTextSize.ExtraLarge => "text-xlarge",
            _ => "text-default"
        };

        var weightClass = textBlock.Weight switch
        {
            AdaptiveTextWeight.Bolder => "text-bold",
            _ => ""
        };

        var colorClass = textBlock.Color switch
        {
            AdaptiveTextColor.Accent => "text-accent",
            AdaptiveTextColor.Attention => "text-attention",
            AdaptiveTextColor.Good => "text-good",
            AdaptiveTextColor.Warning => "text-warning",
            _ => ""
        };

        var classes = $"adaptive-textblock {sizeClass} {weightClass} {colorClass}".Trim();
        
        return $"<div class='{classes}'>{System.Net.WebUtility.HtmlEncode(textBlock.Text)}</div>";
    }

    private string RenderImage(AdaptiveImage image)
    {
        var sizeStyle = image.PixelWidth > 0 ? $"width: {image.PixelWidth}px;" : "";
        if (image.PixelHeight > 0)
        {
            sizeStyle += $" height: {image.PixelHeight}px;";
        }

        var alt = !string.IsNullOrEmpty(image.AltText) 
            ? System.Net.WebUtility.HtmlEncode(image.AltText) 
            : "Adaptive Card Image";

        return $"<div class='adaptive-image'><img src='{System.Net.WebUtility.HtmlEncode(image.Url?.ToString() ?? "")}' alt='{alt}' style='{sizeStyle}' /></div>";
    }

    private string RenderContainer(AdaptiveContainer container)
    {
        var html = "<div class='adaptive-container'>";
        if (container.Items != null)
        {
            foreach (var item in container.Items)
            {
                html += RenderElement(item);
            }
        }
        html += "</div>";
        return html;
    }

    private string RenderColumnSet(AdaptiveColumnSet columnSet)
    {
        var html = "<div class='adaptive-columnset'>";
        if (columnSet.Columns != null)
        {
            foreach (var column in columnSet.Columns)
            {
                html += "<div class='adaptive-column'>";
                if (column.Items != null)
                {
                    foreach (var item in column.Items)
                    {
                        html += RenderElement(item);
                    }
                }
                html += "</div>";
            }
        }
        html += "</div>";
        return html;
    }

    private string RenderFactSet(AdaptiveFactSet factSet)
    {
        var html = "<div class='adaptive-factset'>";
        if (factSet.Facts != null)
        {
            foreach (var fact in factSet.Facts)
            {
                html += "<div class='adaptive-fact'>";
                html += $"<span class='adaptive-fact-title'>{System.Net.WebUtility.HtmlEncode(fact.Title)}:</span> ";
                html += $"<span class='adaptive-fact-value'>{System.Net.WebUtility.HtmlEncode(fact.Value)}</span>";
                html += "</div>";
            }
        }
        html += "</div>";
        return html;
    }

    private string RenderImageSet(AdaptiveImageSet imageSet)
    {
        var html = "<div class='adaptive-imageset'>";
        if (imageSet.Images != null)
        {
            foreach (var image in imageSet.Images)
            {
                html += RenderImage(image);
            }
        }
        html += "</div>";
        return html;
    }

    private string RenderAction(AdaptiveAction action)
    {
        var title = System.Net.WebUtility.HtmlEncode(action.Title ?? "Action");
        
        return action switch
        {
            AdaptiveOpenUrlAction openUrl => 
                $"<a href='{System.Net.WebUtility.HtmlEncode(openUrl.Url?.ToString() ?? "#")}' class='adaptive-action adaptive-action-openurl' target='_blank'>{title}</a>",
            AdaptiveSubmitAction submit => 
                $"<button class='adaptive-action adaptive-action-submit' onclick='alert(\"Submit action not implemented in this context\")'>{title}</button>",
            AdaptiveShowCardAction showCard => 
                $"<button class='adaptive-action adaptive-action-showcard'>{title}</button>",
            _ => $"<button class='adaptive-action adaptive-action-unknown'>{title}</button>"
        };
    }
}
