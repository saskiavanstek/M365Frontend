import React, { useState, useEffect } from 'react';
import './TopHeader.css';

const TopHeader = () => {
  const [darkMode, setDarkMode] = useState(false);

  useEffect(() => {
    document.body.classList.toggle('dark-theme', darkMode);
  }, [darkMode]);

  return (
    <div className="top-header">
      <label className="theme-switch">
        <input 
          type="checkbox" 
          checked={darkMode} 
          onChange={() => setDarkMode(!darkMode)} 
        />
        <span className="slider">
          <span className="icon sun">☀️</span>
          <span className="icon moon">🌙</span>
        </span>
      </label>
    </div>
  );
};

export default TopHeader;
