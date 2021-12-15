import { useState, useEffect } from "react";
import { cqrs } from '../cqrs.js';
import { useReactOidc } from "@axa-fr/react-oidc-context";

export const useEnvironmentUptimes = () => {
    const emptyState = [];
    const [environmentUptimes, setEnvironmentUptimes] = useState({});
    const { oidcUser } = useReactOidc();

    useEffect(() => get(), [0]);

    const get = () => {
        cqrs("/api/schedule/environment-uptimes/get", {}, oidcUser.id_token)
            .then((response) => response.json())
            .then((json) => setEnvironmentUptimes(json.environmentUptimes || emptyState));
    }

    return { environmentUptimes };
};