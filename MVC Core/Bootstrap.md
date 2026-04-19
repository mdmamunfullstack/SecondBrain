# Navbar
## ## 1. Add Bootstrap to your project 
```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Bootstrap Navbar</title>

  <!-- Bootstrap CSS -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body>

  <!-- Your navbar will go here -->

  <!-- Bootstrap JS -->
  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
```

##  2. Create the basic navbar container 
```html
  <nav class="navbar navbar-expand-lg bg-primary">
   <div class="container">
     <a class="navbar-brand" href="#">MyApp</a>
     <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
       <span class="navbar-toggler-icon"></span>
     </button>
     <div class="collapse navbar-collapse" id="navbarNav">
       <ul class="navbar-nav ms-auto">
         <li class="nav-item">
           <a class="nav-link active" href="#">Home</a>
         </li>
         <li class="nav-item">
           <a class="nav-link" href="#">About</a>
         </li>
         <li class="nav-item">
           <a class="nav-link" href="#">Services</a>
         </li>
         <li class="nav-item">
           <a class="nav-link" href="#">Contact</a>
         </li>
       </ul>
     </div>
   </div>
 </nav>
```
### What this means:

- `navbar` → main component
- `navbar-expand-lg` → expands on large screens
- `navbar-light bg-light` → light theme 
- - `ms-auto` → pushes menu to right
- `active` → highlights current page


## Available breakpoints[](https://getbootstrap.com/docs/5.0/layout/breakpoints/#available-breakpoints)

Bootstrap includes six default breakpoints, sometimes referred to as _grid tiers_, for building responsively. These breakpoints can be customized if you’re using our source Sass files.

|Breakpoint|Class infix|Dimensions|
|---|---|---|
|X-Small|_None_|<576px|
|Small|`sm`|≥576px|
|Medium|`md`|≥768px|
|Large|`lg`|≥992px|
|Extra large|`xl`|≥1200px|
|Extra extra large|`xxl`|≥1400px|

## Adding columns to Rows
```html
<div class="container">
  <div class="row">
    <div class="col"> 1 of 2 </div>
    <div class="col"> 2 of 2 </div>
  </div>
  <div class="row"></div>
</div>
```
- Changing the Order of Columns `order-1`
- Leaving Some Space `offset-md-6`
- Adding Gutters `gx-4` 

## Adding Style with Bootstrap
- Heading 
- Text Inline elements
- styles on table 
- working with images
- spacing using bootstrap 
- adding borders 
- bootstrap icons 
- working with colors 
- 

