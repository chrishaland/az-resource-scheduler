import { useState, useEffect } from "react";
import { useFeatureFlagsStore } from "./store";

export const useFeatureFlags = () => {
    const emptyState = { featureFlags: [] };
    const { state } = useFeatureFlagsStore();
    const [featureFlags, setFeatureFlags] = useState(emptyState);

    useEffect(() => setFeatureFlags(state.featureFlags || emptyState));

    return [featureFlags];
};
