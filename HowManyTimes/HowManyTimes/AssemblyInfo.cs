//TODO: 1) zobrazeni poctu counteru v kategorii
//TODO: 2) dlouhe podrzeni plus = otevri submenu (zaloz counter, zaloz kategorii)
//TODO: 3) vy�e�it zalo�en� tabulek p�i prvn�m spu�t�n�!
//               await Database.CreateTableAsync<BaseCounter>().ConfigureAwait(false);
//               await Database.CreateTableAsync<Category>().ConfigureAwait(false);





using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

// Fonts
[assembly: ExportFont("Poppins-Regular.ttf", Alias = "FontPoppinsRegular")]
[assembly: ExportFont("Poppins-Medium.ttf", Alias = "FontPoppinsMedium")]
[assembly: ExportFont("Poppins-SemiBold.ttf", Alias = "FontPoppinsSemiBold")]
[assembly: ExportFont("fontello.ttf", Alias = "FontFontello")]

[assembly: ExportFont("Font Awesome 5 Free-Solid-900.otf", Alias = "FontAwesome5Regular")]