import React, { useEffect, useState } from 'react';
import './ThemeToggle.css'; // We’ll create this in the next step

const ThemeToggle = () => {
  const [isDark, setIsDark] = useState(false);

  useEffect(() => {
    if (isDark) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }, [isDark]);

  const toggleTheme = () => {
    setIsDark(prev => !prev);
  };

  return (
    <div className="theme-toggle" onClick={toggleTheme}>
      <div className={`toggle-slider ${isDark ? 'dark' : 'light'}`}>
        <span className="icon">{isDark ? '🌙' : '🌞'}</span>
      </div>
    </div>
  );
};

export default ThemeToggle;
