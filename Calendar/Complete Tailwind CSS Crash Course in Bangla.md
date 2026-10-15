

url - 
npm

npm init -y 
```
npm install tailwindcss @tailwindcss/cli
```

npm run build
npm run build


```
npm create vite@latest my-react-app
cd my-react-app
npm install
npm install tailwindcss @tailwindcss/vite
npm run dev
``` 


# React + Tailwind CSS Project

A modern React application built with **Vite** and **Tailwind CSS**.

## 🚀 Technologies

- [React](https://react.dev/)
- [Vite](https://vite.dev/)
- [Tailwind CSS](https://tailwindcss.com/)
- JavaScript
- npm
    

## 📁 Project Structure

```text
my-react-app/
├── public/
├── src/
│   ├── App.jsx
│   ├── index.css
│   └── main.jsx
├── index.html
├── package.json
├── vite.config.js
└── README.md
```

## ⚙️ Installation

### 1. Create the React project

```bash
npm create vite@latest my-react-app
```

Select:

```text
Framework: React
Variant: JavaScript
```

### 2. Go to the project directory
```bash
cd my-react-app
```

### 3. Install dependencies
```bash
npm install
```

### 4. InstallTailwind CSS
```bash
npm install tailwindcss @tailwindcss/vite
```

## 🛠️ Tailwind CSS Configuration

Update `vite.config.js`:

```javascript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
})
```

Then update `src/index.css`:

```css
@import "tailwindcss";
```

## ▶️ Run the Project

Start the development server:

```bash
npm run dev
```

The application will be available at:

```text
http://localhost:5173
```

## 🏗️ Build for Production

Create a production build:

```bash
npm run build
```

Preview the production build:

```bash
npm run preview
```

## 🎨 Example Tailwind Usage

```jsx
function App() {
  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="bg-white p-8 rounded-xl shadow-lg text-center">
        <h1 className="text-4xl font-bold text-blue-600">
          Hello React + Tailwind!
        </h1>

        <p className="mt-4 text-gray-600">
          Tailwind CSS is working 🎉
        </p>

        <button className="mt-6 px-5 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          Click Me
        </button>
      </div>
    </div>
  )
}

export default App
```

## 📜 Available Commands

|Command|Description|
|---|---|
|`npm run dev`|Start development server|
|`npm run build`|Build application for production|
|`npm run preview`|Preview production build|

## 📚 Resources

- [React Documentation](https://react.dev/)
- [Vite Documentation](https://vite.dev/guide/)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)

## 📄 License

This project is available for personal and educational use.