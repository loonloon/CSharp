import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import { store } from './actions/Store.jsx';
import App from './components/App.jsx';

/**
 * @class The main react app with the redux store.
 */
class ReduxApp extends React.Component{
    render() {
        return (
            <Provider store={store}>
                <App />
            </Provider>
        );
    }
}

render(<ReduxApp />, document.getElementById("root"));