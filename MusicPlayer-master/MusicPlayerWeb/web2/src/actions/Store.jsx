
import {  applyMiddleware, combineReducers, createStore } from 'redux';
import { currentSong } from '../reducers/CurrentSong.jsx';
import { serverInfo } from '../reducers/ServerInfo.jsx';
import { copyProgress } from '../reducers/Copy.jsx';

/**
 * @desc The initial redux state.
 */
const initialState = {
    currentSong: null,
    serverInfo: null,
    copyProgress: null
};

/**
 * @desc Creates the redux store.
 */
export function configureStore() {  
    let reducers = combineReducers({ currentSong, serverInfo, copyProgress });
    const store = createStore(reducers, initialState);
    return store;
}

export const store = configureStore(); 
document.ReduxStore = store;
