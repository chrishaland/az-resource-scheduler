import { useLanguage } from "./store";

const getTranslatedString = (locales, language, id) => {
    const string = locales?.[language]?.[id];
    return string;
};

export const Locale = (props) => {
    const { locales, id, children } = props;

    const { state } = useLanguage();
    return getTranslatedString(locales, state.language, id) || children;
};
