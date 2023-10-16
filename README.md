# LPaper
this is a barebones diy wallpaper engine, that lets the user render about anything as a windows wallpaper.
it uses a webview as a engine to render anything that can be done in a webbrowser.


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
