const path = require('path');

module.exports = {
  entry: './index.js',
  output: {
    filename: 'sidebar.bundle.js',
    path: path.resolve(__dirname, '../wwwroot/react'),
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
        },
      },
      {
        test: /\.css$/,
        use: ['style-loader', 'css-loader'],
      },
    ],
  },
  resolve: {
    extensions: ['.js', '.jsx'],  // Add .jsx here for React components
    alias: {
      ReactSidebar: path.resolve(__dirname, 'ReactSidebar/'),
    },
  },
  mode: 'development',
};
