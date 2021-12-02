import { useState, useEffect } from "react";
import { cqrs } from '../cqrs.js';
import { useReactOidc } from "@axa-fr/react-oidc-context";

export const useTimeZones = () => {
    const emptyState = [{id: "W. Europe Standard Time"}];
    const [timeZones, setTimeZones] = useState(emptyState);
    const { oidcUser } = useReactOidc();

    useEffect(() => get(), [0]);

    const get = () => {
        cqrs("/api/timezones/get", {}, oidcUser.id_token)
            .then((response) => response.json())
            .then((json) => setTimeZones(json.timeZones || emptyState));
    }

    return [timeZones];
};
