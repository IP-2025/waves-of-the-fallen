import {Column, Entity, ManyToOne, PrimaryGeneratedColumn} from 'typeorm';
import {Player} from './Player';
import {Character} from './Character';

@Entity()
export class UnlockedCharacter {

  @PrimaryGeneratedColumn()
  id!: number;

  @ManyToOne(() => Player, (player) => player.player_id, { nullable: false,  onDelete: 'CASCADE' })
  player!: Player;

  @ManyToOne(() => Character, (character) => character.character_id, { nullable: false,  onDelete: 'CASCADE' })
  character!: Character;

  @Column({
    type: 'int',
    nullable: false,
  })
  level!: number;
}