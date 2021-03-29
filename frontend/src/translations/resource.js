import { useLanguage } from "./store";

const getTranslatedString = (resources, language, id) => {
    const string = resources?.[language]?.[id];
    return string;
};

export const Resource = (props) => {
    const { resources, id, children } = props;

    const { state } = useLanguage();
    return getTranslatedString(resources, state.language, id) || children;
};
