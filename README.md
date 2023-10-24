# LPaper
this is a barebones diy wallpaper engine, that lets the user render about anything as a windows wallpaper.
it uses a webview as a engine to render anything that can be done in a webbrowser.

![jinx webm wallpaper example](example.gif)

## About this
first attempt to do this, it is very barebones and only let you apply the wallpaper on your main monitor at the moment.

it creates a webview on the layer behind the desktop/icons to show you what is simply a website. How to choose what to show, edit the index.html file.
Don't forget to add these in css:

```css
html {
  height: 100vh;
  width: 100vw;
}
```

this is to make sure the wallpaper covers your screen.

### Sad limitations,
sadly it doesnt support mp4 files since it requires a license. Convert those files to webm. This can be done using ffmpeg or [CloudConvert]([cloudconvert](https://cloudconvert.com/))

### Do not panic
if you cant find where to revert this, just find your way to the "system tray" (the little arrow close to your clock and date in windows) and right click on the little heart icon to close. Either that or just restart your pc.
