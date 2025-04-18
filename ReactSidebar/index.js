import React from 'react';
import { createRoot } from 'react-dom/client';
import Sidebar from './Sidebar';
import './Sidebar.css';

const container = document.getElementById('react-sidebar');
const root = createRoot(container);
root.render(<Sidebar />);
