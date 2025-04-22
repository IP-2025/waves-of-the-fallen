import { Character } from "../../src/libs/entities/Character";
import {innitAllCharacters } from "../../src/services/characterService";
import { getAllCharacters } from "../../src/services/characterService";

describe('Character tests', () => {
    it('Initial Save', async () => {

        const mockCharacters: Character[] = [
            {
                characterId: 1,
                name: 'TestChar',
                speed: 10,
                health: 100,
                dexterity: 20,
                intelligence: 30
            }
        ];

        await innitAllCharacters(mockCharacters);

        const allCharacters = await getAllCharacters();

        expect(allCharacters.length).toBe(mockCharacters.length);
        expect(allCharacters[0].characterId).toBe(mockCharacters[0].characterId);
        expect(allCharacters[0].name).toBe(mockCharacters[0].name);
        expect(allCharacters[0].speed).toBe(mockCharacters[0].speed);
        expect(allCharacters[0].health).toBe(mockCharacters[0].health);
        expect(allCharacters[0].dexterity).toBe(mockCharacters[0].dexterity);
        expect(allCharacters[0].intelligence).toBe(mockCharacters[0].intelligence);
    });

    it('Load new Characters', async () => {
        let mockCharacters: Character[] = [
            {
                characterId: 1,
                name: 'TestChar',
                speed: 10,
                health: 100,
                dexterity: 20,
                intelligence: 30
            }
        ];

        await innitAllCharacters(mockCharacters);

        const allCharacters = await getAllCharacters();

        expect(allCharacters.length).toBe(mockCharacters.length);
        expect(allCharacters[0].characterId).toBe(mockCharacters[0].characterId);
        expect(allCharacters[0].name).toBe(mockCharacters[0].name);
        expect(allCharacters[0].speed).toBe(mockCharacters[0].speed);
        expect(allCharacters[0].health).toBe(mockCharacters[0].health);
        expect(allCharacters[0].dexterity).toBe(mockCharacters[0].dexterity);
        expect(allCharacters[0].intelligence).toBe(mockCharacters[0].intelligence);

       mockCharacters = [
            {
                characterId: 2,
                name: 'TestChar2',
                speed: 20,
                health: 200,
                dexterity: 30,
                intelligence: 40
            }
        ];

        await innitAllCharacters(mockCharacters);

        const allCharacters2 = await getAllCharacters();

        expect(allCharacters2.length).toBe(mockCharacters.length);
        expect(allCharacters2[0].characterId).toBe(mockCharacters[0].characterId);
        expect(allCharacters2[0].name).toBe(mockCharacters[0].name);
        expect(allCharacters2[0].speed).toBe(mockCharacters[0].speed);
        expect(allCharacters2[0].health).toBe(mockCharacters[0].health);
        expect(allCharacters2[0].dexterity).toBe(mockCharacters[0].dexterity);
        expect(allCharacters2[0].intelligence).toBe(mockCharacters[0].intelligence);
    });
})
