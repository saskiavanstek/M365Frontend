import React, { useState, useEffect } from "react";
import "./Sidebar.css"; // Ensure to include your CSS styles

const Sidebar = () => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [activeDropdown, setActiveDropdown] = useState(null);

  const toggleSidebar = () => {
    setIsCollapsed(!isCollapsed);
  };

  const toggleDropdown = (index) => {
    setActiveDropdown(activeDropdown === index ? null : index);
  };

  // 🟡 NEW: Add or remove a class on the body when sidebar toggles
  useEffect(() => {
    if (isCollapsed) {
      document.body.classList.remove("sidebar-expanded");
    } else {
      document.body.classList.add("sidebar-expanded");
    }
  }, [isCollapsed]);

  return (
    <div className={`sidebar ${isCollapsed ? "collapsed" : ""}`}>
      <div className="sidebar-header">
        <h3 className="brand">
          <i className="fa fa-laptop"></i>
          <span>IT-M365</span>
        </h3>
        <div className="toggle-btn" onClick={toggleSidebar}>
          <i
            className={`fas ${
              isCollapsed ? "fa-chevron-right" : "fa-chevron-left"
            } toggle-icon`}
          ></i>
        </div>
      </div>
      <ul className="nav-links">
        <li>
          <a href="/Index" className="nav-item">
            <span className="nav-icon">
              <i className="fas fa-home"></i>
            </span>
            <span>Home</span>
          </a>
        </li>
        <li>
          <a href="/Trainer" className="nav-item">
            <span className="nav-icon">
              <i className="fas fa-user"></i>
            </span>
            <span>Links aanmaken</span>
          </a>
        </li>
        <li>
          <a href="Klanten/Create" className="nav-item">
            <span className="nav-icon">
              <i className="fas fa-user"></i>
            </span>
            <span>KLant Aanmaken</span>
          </a>
        </li>
        <li>
          <a href="/Allelinks" className="nav-item">
            <span className="nav-icon">
              <i className="fa fa-search"></i>
            </span>
            <span>Links Zoeken</span>
          </a>
        </li>
        <li>
          <a href="/Trainer/Calendar" className="nav-item">
            <span className="nav-icon">
              <i className="fa fa-calendar"></i>
            </span>
            <span>Agenda View Links</span>
          </a>
        </li>
      </ul>
    </div>
  );
};

export default Sidebar;
