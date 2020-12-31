using Xamarin.Forms.Xaml;
using Blastic.CodeGeneration;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Preserve(AllMembers = true)]
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: CreateLocalizableProperties("Blastic.Forms.Sample.Resources")]
[assembly: CreateLocalizationSource("Blastic.Forms.Sample.Resources")]

[assembly: ExportFont("MaterialIcons-Regular.ttf", Alias = "Material")]
[assembly: ExportFont("Roboto-Regular.ttf", Alias = "Roboto")]
[assembly: ExportFont("Roboto-Bold.ttf", Alias = "RobotoBold")]
