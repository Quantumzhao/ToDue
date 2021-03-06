# ToDue

## What is it?

It's basically a todo list. 

Originally I planned to implement it as a *Rainmeter* widget, but failed for all sorts of reasons. 

## What are the specialties?

- It is simple and concise
- It pins on desktop just like a *Windows 7* gadget. Great for reminding people (especially me) that they have works to do
- It also shows the due dates

## Screenshots

### Initial design

<img src="design.jpg" alt="design" style="zoom:50%;" />

### Actual

<img src="actual.jpg" alt="actual" style="zoom:50%;" />

## Other

- Require `Source Han Sans` font installed for best appearance

    This is an *OpenType* font, so I cannot embed it into the executable. 

- Create a shortcut in `Startup` folder to make it automatically start on *Windows* start. 

- The font `segmdl2` is copyrighted by *Microsoft*

- The font `CONSERVATIVE SIMPLICITY` is created by myself. It is not allowed for commercial purposes. For more copyright info, please visit my *Behance* page

---

## Checklist

- [x] Transparent background
- [x] Title
- [x] Todo item data structure
- [x] List view of items
- [x] Add button
- [x] Separator
- [x] Input textboxes
- [x] Handling adding new items
    - [x] Building domain model from inputs
    - [x] Adding & Sorting
- [x] Handling removing items
- [x] Draggable interface
- [x] Keep on desktop
- [x] Show in notification area
    - [x] Setting transparency
    - [x] Switching between light/dark mode
    - [x] Exit
- [x] An icon
- [x] Read from/write to file
- [x] Embed fonts
- [ ] Start on bootup