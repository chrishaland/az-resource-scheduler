import { useState, useEffect } from "react";
import { useAccountStore } from "./store";
import { cqrs } from '../cqrs.js';

export const useAccount = () => {
    const emptyState = { givenName: "", surName: "", email: "", roles: [], name: "" };

    const { state, dispatch } = useAccountStore();
    const [account, setAccount] = useState(emptyState);

    useEffect(() => setAccount(state.account || emptyState));

    const refetchAccount = () => {
        cqrs('/api/account/get', {})
            .then((response) => response.json())
            .then((json) => dispatch({ type: "setAccount", account: json.account }))
            .catch(() => dispatch({ type: "setAccount", account: {} }));
    }

    return [account, refetchAccount];
};
