using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using CreationEditor.Avalonia.ViewModels.Scripting;
using ReactiveUI;
using TextMateSharp.Grammars;
namespace CreationEditor.Avalonia.Views.Scripting;

public partial class ScriptEditor : ActivatableUserControl {
    public static readonly StyledProperty<ObservableCollection<ThemeName>> ThemeNamesProperty
        = AvaloniaProperty.Register<ScriptEditor, ObservableCollection<ThemeName>>(
            nameof(ThemeNames),
            new ObservableCollection<ThemeName>(Enum.GetValues<ThemeName>()));

    public ObservableCollection<ThemeName> ThemeNames {
        get => GetValue(ThemeNamesProperty);
        set => SetValue(ThemeNamesProperty, value);
    }

    public static readonly StyledProperty<ThemeName> CurrentThemeProperty
        = AvaloniaProperty.Register<ScriptEditor, ThemeName>(nameof(CurrentTheme), ThemeName.HighContrastDark);

    public ThemeName CurrentTheme {
        get => GetValue(CurrentThemeProperty);
        set => SetValue(CurrentThemeProperty, value);
    }

    private readonly AvaloniaTextMateRegistryOptions _textMateRegistryOptions;
    private readonly TextMate.Installation _textMateInstallation;

    public ScriptEditor() {
        InitializeComponent();

        // Configure caret
        Editor.TextArea.RightClickMovesCaret = true;
        this.WhenActivated(d => { 
            Editor.TextArea.Caret.PositionChanged += UpdateCaretPosition;
            d.Add(Disposable.Create(() => Editor.TextArea.Caret.PositionChanged -= UpdateCaretPosition));
        });

        // Setup syntax highlighting
        _textMateRegistryOptions = new AvaloniaTextMateRegistryOptions(CurrentTheme, "Papyrus");

        _textMateInstallation = Editor.InstallTextMate(_textMateRegistryOptions);
        var papyrusGrammar = _textMateRegistryOptions.GetGrammarDefinition("papyrus");
        if (papyrusGrammar is null) throw new Exception();

        _textMateInstallation.SetGrammar(papyrusGrammar.Contributes.Grammars[0].ScopeName);

        // Example document
        Editor.Document = new TextDocument(
            "; AvaloniaEdit supports displaying control chars: \a or \b or \v" + Environment.NewLine +
            "; AvaloniaEdit supports displaying underline and strikethrough" + Environment.NewLine +
            """
            Scriptname BSKWICourierScript extends Quest

            Import Debug

            Function Test()
                while (x < 4)
            	    x += 1
                endwhile
                
            	if (x < 0)
            		x *= 2
            		x = 2 / 2
            		x = 2 + 2
            	endif
            
            	int i = 0
            	int s = 123
            
            	if (i < 0)
            		tesat(asdf, asdf)
            		asdfklj(sadf, 0)
            		i %= 0
            		asdfsadf
            	elseif (i > 0)
            		asdf()
            	else
            		asdf()
            	endif
            EndFunction

            Event X(int _x, Armor yasdf21) native

            Event OnLoad(int _x, Armor yasdf21)
             	asdf.tes()
            EndEvent

            ObjectReference Function SendLetter(Book letter)
            	ObjectReference letterRef = GetCourierContainer().PlaceAtMe(letter)
            	AddRefToContainer(letterRef)
            	return letterRef
            EndFunction

            Auto State Test
                Event OnInit()
                    Debug.Trace("Test")
                EndEvent
            EndState

            ReferenceAlias Property LocationCenterMarker Auto
            Int Property Test = 0 AutoReadOnly
            ReferenceAlias Property CourierMarker Auto

            Actor Property CourierRef Auto
            {Courier placed actor}

            Actor Property PlayerRef Auto

            Function TellMainQuestImDown()
            	(a() + a) < 0
            	(asdf() as asdf).asdf()
            	(a() + a).asdf()
            	(a() as b).asdf()
            	(GetOwningQuest() as ArenaCombatQuest).OpponentDowned()
            EndFunction

            Event OnEnterBleedout()
            	if ((GetOwningQuest() as ArenaCombatQuest).IsBluntedQuest)
            		GetActorReference().StopCombat()
            		TellMainQuestImDown()
            	endif
            EndEvent

            ;================================================================================
            ;	API FUNCTIONS
            ;================================================================================

            ObjectReference Function SendLetter(Book letter)
            	ObjectReference letterRef = GetCourierContainer().PlaceAtMe(letter)
            	AddRefToContainer(letterRef)
            	return letterRef
            EndFunction

            Function SendForm(Form item, int count = 1)
            	AddItemToContainer(item, count)
            	return
            EndFunction

            Function CancelItemRef(ObjectReference item)
            	RemoveRefFromContainer(item)
            EndFunction
            """);

        // Handle zooming
        AddHandler(PointerWheelChangedEvent, (o, i) => {
            if (i.KeyModifiers != KeyModifiers.Control) return;

            if (i.Delta.Y > 0) {
                Editor.FontSize++;
            } else {
                Editor.FontSize = Editor.FontSize > 1 ? Editor.FontSize - 1 : 1;
            }
        }, RoutingStrategies.Bubble, true);
    }

    public ScriptEditor(IScriptVM scriptVM) : this() {
        DataContext = scriptVM;
    }

    protected override void WhenActivated() {
        this.WhenAnyValue(x => x.CurrentTheme)
            .Subscribe(theme => _textMateInstallation.SetTheme(_textMateRegistryOptions.LoadTheme(theme)));
    }

    private void UpdateCaretPosition(object? o, EventArgs eventArgs) {
        StatusText.Text = $"Line {Editor.TextArea.Caret.Line} Column {Editor.TextArea.Caret.Column}";
    }
}
