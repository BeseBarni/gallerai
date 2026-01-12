import js from '@eslint/js'
import prettierConfig from 'eslint-config-prettier'
import reactCompiler from 'eslint-plugin-react-compiler'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tailwind from 'eslint-plugin-tailwindcss'
import { defineConfig, globalIgnores } from 'eslint/config'
import globals from 'globals'
import tseslint from 'typescript-eslint'

export default defineConfig([
  globalIgnores(['dist', 'src/shadcn/**/*.{ts,tsx}']),
  {
    files: ['**/*.{ts,tsx}'],
    plugins: {
      tailwindcss: tailwind,
    },
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs.flat.recommended,
      reactRefresh.configs.vite,
      reactCompiler.configs.recommended,
    ],
    rules: {
      ...tailwind.configs['flat/recommended'][0].rules,
      'tailwindcss/no-custom-classname': 'off',
    },
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
    },
    settings: {
      tailwindcss: {
        callees: ['cn', 'cva'],
        config: 'src/index.css',
      },
    },
  },
  prettierConfig,
])
