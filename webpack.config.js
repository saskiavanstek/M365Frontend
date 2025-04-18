const path = require('path');

module.exports = {
    mode: 'development', // Of 'production' voor een productie build
    entry: {
        sidemenu: './wwwroot/js/react/sidemenu.jsx',
        header: './wwwroot/js/react/header.jsx'
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot/js/react'),
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env', '@babel/preset-react']
                    }
                }
            },
            {
                test: /\.css$/i,
                use: ["style-loader", "css-loader"],
            },
        ],
    },
    resolve: {
        extensions: ['.js', '.jsx'],
    },
};